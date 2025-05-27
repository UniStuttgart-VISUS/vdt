// <copyright file="RunCommandCustomisation.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.XPath;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Validation;


namespace Visus.DeploymentToolkit.Unattend {

    /// <summary>
    /// Adds a &quot;run command&quot; or &quot;run async command&quot; step to
    /// the untttend.xml file.
    /// </summary>
    public sealed class RunCommandCustomisation(
            IUnattendBuilder builder,
            ILogger<RunCommandCustomisation> logger)
            : CustomisationBase(logger) {

        #region Public properties
        /// <summary>
        /// Gets or sets a description of the command.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the command should run asynchronously.
        /// </summary>
        public bool IsAsynchronous { get; set; } = false;

        /// <summary>
        /// Gets or sets the order of the command.
        /// </summary>
        public uint Order { get; set; } = 1;

        /// <summary>
        /// Gets or sets the path to the command to run.
        /// </summary>
        [Required]
        [FileExists]
        public string Path { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override void Apply(XDocument unattend) {
            var resolver = GetResolver(unattend);
            var settings = unattend.XPathSelectElements(this.SettingsFilter,
                resolver);

            // Create a filter for the group holding the commands and the
            // command elements themselves.
            var commandName = this.IsAsynchronous
                ? "RunAsynchronousCommand"
                : "RunSynchronousCommand";
            var groupName = this.IsAsynchronous
                ? "RunAsynchronous"
                : "RunSynchronous";
            var groupFilter = $"//u:{groupName}";
            this._logger.LogTrace("The command group is \"{Group}\" and will "
                + "be selected using the XPath expression \"{Filter}\".",
                groupName, groupFilter);

            foreach (var s in settings) {
                var group = s.XPathSelectElement(groupFilter, resolver);

                // If the (a)synchronous group does not exist, we create it. As
                // the group could be missing because of the containing
                // component not being present, we must check this first.
                if (group is null) {
                    var componentFilter = GetComponentFilter(
                        Components.WindowsSetup);
                    var component = s.XPathSelectElement(componentFilter,
                        resolver);
                    if (component is null) {
                        this._logger.LogError("The component \"{Component}\" " 
                           + "containing the command was not found using the "
                           + "XPath expression \"{Filter}\", so we add it now.",
                           Components.WindowsSetup, componentFilter);
                        component = this._builder.MakeComponent(
                            Components.WindowsSetup, null);
                        s.Add(component);
                    }
                    Debug.Assert(component is not null);

                    this._logger.LogTrace("The command group \"{Group}\" was "
                        + "not found using the XPath expression \"{Filter}\", "
                        + " so we add it now.", groupName, groupFilter);
                    group = new XElement(this._builder.MakeName(groupName));
                    component.Add(group);
                } /* if (group is null) */
                Debug.Assert(group is not null);

                var dupeFilter = $"//u:{commandName}/u:{OrderElement}"
                    + $"[contains(text(), \"{this.Order}\")]";
                var duplicate = group.XPathSelectElement(dupeFilter, resolver);
                if (duplicate is not null) {
                    duplicate = duplicate.Parent;
                    Debug.Assert(duplicate is not null);
                    var path = duplicate.Descendant(PathElement)?.Value;

                    this._logger.LogWarning("A duplicate command \"{Command}\" "
                        + "was identified using the XPath expression "
                        + "\"{Filter}\". This command will be replaced. If "
                        + "both commands are expected to run, assign different "
                        + "orders to them.", path, dupeFilter);
                    duplicate.Remove();
                }
                Debug.Assert(group.XPathSelectElement(dupeFilter, resolver)
                    is null);

                this._logger.LogTrace("Adding {Command} running \"{Path}\".",
                    commandName, this.Path);
                var command = new XElement(this._builder.MakeName(commandName),
                    new XElement(this._builder.MakeName(DescriptionElement),
                        this.Description),
                    new XElement(this._builder.MakeName(OrderElement),
                        this.Order.ToString()),
                    new XElement(this._builder.MakeName(PathElement),
                    this.Path));
                group.Add(command);
            }
        }
        #endregion

        #region Private constants
        private const string DescriptionElement = "Description";
        private const string OrderElement = "Order";
        private const string PathElement = "Path";
        #endregion

        #region Private fields
        private readonly IUnattendBuilder _builder = builder
            ?? throw new ArgumentNullException(nameof(builder));
        #endregion
    }
}
