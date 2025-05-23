# Project Deimos
Project Deimos aims to provide a partial replacement of the [Microsoft Deployment Toolkit (MDT)](https://learn.microsoft.com/de-de/mem/configmgr/mdt/) for internal use at VISUS. The main motivation for this project are that (i) MDT seems to be abandoned by Microsoft and (ii) is based on VBS which is a deprecated feature in Windows 11. Project Deimos does not provide a full replacement for MDT, but only for the deployment tasks performed at VISUS.

## Components
The project consists of several components that limit the number of times the PXE image needs to be updated. This is mainly achieved by running the bulk of the deployment tasks from the deployment share and having a small bootstrapper that only mounts the share.

### Visus.DeploymentToolkit.Bootstrapper
This is a minimal application that mainly mounts the deployment share into WinPE and starts the agent from the share. The boostrapper depends on `Visus.DeploymentToolkit.Bootstrapping`, which contains tasks that need to be performed by the boostrapper, but might also be used by the agent itself. Generally, as few tasks as possible should be placed here, because changes in the bootstrapper and its library require the PXE image to be rebuilt.

### Visus.DeploymentToolkit.Agent
The agent performs the installation from WinPE and finalises it when booting into the installed operating system from the disk. The agent can use the tasks from `Visus.DeploymentToolkit.Bootstrapping` as well as the main deployment tasks defined in `Visus.DeploymentToolkit`. The steps performed by the agent include:

1. Partitioning the disk using the VDS.
1. Formatting the partitions using the VDS.
1. Applying the image to the disk using DISM.
1. Configuring the boot manager using BCDEdit.
1. Rebooting into the installed operating system.
1. Applying the unattend.xml file to the installed operating system.

### Visus.DeploymentToolkit.ImageBuilder
The image builder is intended to create images to be served by the TFTP server.

### Visus.DeploymentToolkit.TaskRunner
The task runner tool allows for executing individual tasks from the deployment toolkit. The application needs the following parameters, either via the command line or via appsettings.json: The `StateFile` where tasks relying on existing state can obtain this information from, and the `Task`, which is the class name of the work item to be executed. Parameters to the tasks (public properties of the task object) can be set via the `Parameters` configuration section.

For instance, one can create a deployment share via the `Visus.DeploymentToolkit.Tasks.PrepareDeploymentShare` task like this:
```powershell
Visus.DeploymentToolkit.TaskRunner.exe /Task=PrepareDeploymentShare /Parameters:Path=d:\DeploymentShare
```

## Development
### Tasks
All steps that can be executed by Project Deimos must implement the [`Visus.DeploymentToolkit.ITask`](Visus.DeploymentToolkit.Contracts/Tasks/ITask.cs) interface. Typically, this is achieved by inheriting from [`Visus.DeploymentToolkit.Tasks.TaskBase`](Visus.DeploymentToolkit.Bootstrapping/Tasks/TaskBase.cs). Tasks are configured via their public properties. The actual work is performed within `ExecuteAsync`.

> [!IMPORTANT]
> Implementations should avoid performing blocking operations and just return `Task.CompletedTask`. Start new tasks manually when performing long-running operations that are not inherently asynchronous.

> [!IMPORTANT]
> Tasks should be placed in `Visus.DeploymentToolkit` instead of `Visus.DeploymentToolkit.Bootstrapping` whenever possible. Adding tasks to the bootstrapping library increases the size of the boot image and requires the image to be recreated for them to become available. Adding tasks in the main library allows the bootstrapper to download them from the network without the need to regenerate the images.

> [!TIP]
> Use the [`Visus.DeploymentToolkit.Tasks.SupportsPhaseAttribute`](Visus.DeploymentToolkit.Bootstrapping/Tasks/SupportsPhaseAttribute.cs) to have `CanExecute` implemented automatically.

> [!TIP]
> Use the [`Visus.DeploymentToolkit.Extensions.FromStateAttribute`](Visus.DeploymentToolkit.Bootstrapping/Extensions/FromStateAttribute.cs) and the [`Visus.DeploymentToolkit.Extensions.ObjectExtensions.CopyFrom`](Visus.DeploymentToolkit.Bootstrapping/Extensions/ObjectExtensions.cs) extension method to set properties of a task from the injected state. This enables previous tasks to pass on data to their successors.
 
### Task Sequences
Task sequences are usually authored by end users in the form of JSON files that are deserialised by the [`Visus.DeploymentToolkit.Workflow.ITaskSequenceStore`](Visus.DeploymentToolkit.Contracts/Workflow/ITaskSequenceStore.cs). However, task sequences can also be created programmatically like this:
```c#
ITaskSequenceFactory tasks; // This should be injected from the DI.

tasks.CreateBuilder()
    .ForPhase(Phase.PreinstalledEnvironment)
    .Add<CopyWindowsPe>()
    .Add<MountWim>()
    .Add<CopyFiles>((t, s) => {
        ArgumentNullException.ThrowIfNull(s.DeploymentShare);
        ArgumentNullException.ThrowIfNull(s.WimMount);
        t.Source = Path.Combine(s.DeploymentShare!, DeploymentShare.Layout.BootstrapperPath);
        t.Destination = Path.Combine(s.WimMount.MountPoint, "deimos");
        t.IsRecursive = true;
        t.IsRequired = true;
        t.IsCritical = true;
    })
    .Add<UnmountWim>()
    .Add<CreateWindowsPeIso>()
    .Build();
```
