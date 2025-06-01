// <copyright file="WmiBcdStore.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.IO;
using System.Management;
using System.Runtime.Versioning;
using Visus.DeploymentToolkit.Bcd.Properties;


namespace Visus.DeploymentToolkit.Bcd {

    /// <summary>
    /// A WMI implementation for manipulating a BCD store.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class WmiBcdStore : IBcdStore {

        #region Factory methods
        /// <summary>
        /// Creates a new BCD store at the specified location and opens it.
        /// </summary>
        /// <param name="path">The path where the new BCD store is stored.
        /// </param>
        /// <returns>An object representing the newly BCD store.</returns>
        /// <exception cref="InvalidOperationException">If the store could not
        /// be created. This is typically the case if the file designated by
        /// <paramref name="path"/> already exists.</exception>
        public static WmiBcdStore Create(string path) {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            path = Path.GetFullPath(path);

            var bcd = new ManagementClass(WmiScope.Value,
                new ManagementPath(BcdStoreClass),
                null);

            var args = bcd.GetMethodParameters("CreateStore");
            args["File"] = path;

            var result = bcd.InvokeMethod("CreateStore", args, null);
            if (!(bool) result["ReturnValue"]) {
                throw new InvalidOperationException(string.Format(
                    Errors.FailedCreateBcdStore, path));
            }

            return new WmiBcdStore(path);
        }
        #endregion

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="options">The options used when opending the management
        /// object representing the default store.</param>
        public WmiBcdStore(ObjectGetOptions? options = null)
            : this(string.Empty, options) { }

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="path">The path to the BCD store to open.</param>
        /// <param name="options">The options used when opening the management
        /// object.</param>
        public WmiBcdStore(string path, ObjectGetOptions? options = null) {
            ArgumentNullException.ThrowIfNull(path);
            this._store = new ManagementObject(WmiScope.Value,
                new ManagementPath($"{BcdStoreClass}.FilePath='{path}'"),
                options);
        }
        #endregion

        #region Private constants
        /// <summary>
        /// The WMI class representing a BCD store.
        /// </summary>
        private const string BcdStoreClass = "BcdStore";

        /// <summary>
        /// The management scope where the BCD stuff is located.
        /// </summary>
        private static readonly Lazy<ManagementScope> WmiScope = new (() => {
            var retval = new ManagementScope(@"\\.\root\wmi");
            retval.Connect();
            return retval;
        });
        #endregion

        #region Private fields
        private ManagementObject _store;
        #endregion
    }
}
