# Visus.DeploymentToolkit.Bcd
This library provides a C# interface to the [boot configuration data store](https://learn.microsoft.com/en-us/previous-versions/windows/desktop/bcd/bcdstore). The BCD store is rather poorly documented. Most of the code is derived from an existing [Powershell implementation](https://github.com/mattifestation/BCD/) and a [website on BCD](https://www.geoffchappell.com/notes/windows/boot/bcd/index.htm?tx=36).

> [!WARNING]
> The library is a very bare-bones interface to BCD that only exposes the minimum we need for our tasks.
