# Visus.DeploymentToolkit.Vds
This library provides a C# interface to the [Virtual Disk Service (VDS)](https://learn.microsoft.com/en-us/windows/win32/vds/virtual-disk-service-portal) API. This API is used for low-level diskpart-style operations to prepare a target system.

> [!WARNING]  
> The library is a very bare-bones interface to VDS that only exposes the minimum we need for our tasks. Some methods of the COM interface are not mapped correctly, because we do not need them. Calling these methods will most likely crash the application. 