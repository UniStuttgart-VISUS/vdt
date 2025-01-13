# Project Deimos
Project Deimos aims to provide a partial replacement of the [Microsoft Deployment Toolkit (MDT)](https://learn.microsoft.com/de-de/mem/configmgr/mdt/) for internal use at VISUS. The main motivation for this project are that (i) MDT seems to be abandoned by Microsft and (ii) is based on VBS which is a deprecated feature in Windows 11. Project Deimos does not provide a full replacement for MDT, but only for the deployment tasks performed at VISUS.

## Components
The project constists of serveral components that limit the number of times the PXE image needs to be updates. This is mainly achieved by running the bulk of the deployment tasks from the deployment share and having a small bootstrapper that only mounts the share.

### Visus.DeploymentToolkit.Bootstrapper
This is a minimal application that mainly mounts the deployment share into WinPE and starts the agent from the share. The boostrapper depends on `Visus.DeploymentToolkit.Bootstrapping`, which contains tasks that need to be performed by the boostrapper, but might also be used by the agent itself. Generally, as few tasks as possible should be placed here, because changes in the bootstrapper and its library require the PXE image to be rebuilt.

### Visus.DeploymentToolkit.Agent
The agent performs the installation from WinPE and finalises it when booting into the installed operating system from disk.
