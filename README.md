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
The image builder is intended for creates the images to be services by the TFTP server.

### Visus.DeploymentToolkit.TaskRunner
The task runner tool allows for executing individual tasks from the deployment toolkit. The application needs the following parameters, either via the command line or via appsettings.json: The `StateFile` where tasks relying on existing state can obtain this information from, and the `Task`, which is the class name of the work item to be executed. Parameters to the tasks (public properties of the task object) can be set via the `Parameters` configuration section.

For instance, one can create a deployment share via the `Visus.DeploymentToolkit.Tasks.PrepareDeploymentShare` task like this:
```powershell
Visus.DeploymentToolkit.TaskRunner.exe /Task=PrepareDeploymentShare /Parameters:Path=d:\DeploymentShare
```
