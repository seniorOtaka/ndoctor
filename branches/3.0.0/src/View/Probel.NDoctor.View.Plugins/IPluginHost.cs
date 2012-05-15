﻿/*
    This file is part of NDoctor.

    NDoctor is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    NDoctor is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with NDoctor.  If not, see <http://www.gnu.org/licenses/>.
*/
namespace Probel.NDoctor.View.Plugins
{
    using System;
    using System.Windows.Controls;

    using Probel.NDoctor.Domain.DTO.Helpers;
    using Probel.NDoctor.Domain.DTO.Objects;
    using Probel.NDoctor.View.Plugins.Helpers;
    using Probel.NDoctor.View.Plugins.MenuData;

    public interface IPluginHost
    {
        #region Properties

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in back navigation
        ///  history.
        /// </summary>
        /// <value>
        ///  <c>true</c> if there is at least one entry in back navigation history; otherwise, <c>false</c>.
        /// </value>
        bool CanNavigateBack
        {
            get;
        }

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in forward
        /// navigation history.
        /// </summary>
        /// <value>
        /// <c>true</c> if there is at least one entry in forward navigation history; otherwise,<c>false</c>.
        /// </value>
        bool CanNavigateForward
        {
            get;
        }

        /// <summary>
        /// Gets or sets the connected user.
        /// If set to null, it means no user are connected into nDoctor
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        LightUserDto ConnectedUser
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the version of the host. It is used for the plugin validation.
        /// </summary>
        Version HostVersion
        {
            get;
        }

        /// <summary>
        /// Gets or sets the selected patient.
        /// </summary>
        /// <value>
        /// The patient.
        /// </value>
        LightPatientDto SelectedPatient
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the workday.
        /// </summary>
        Workday Workday
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds the specified tab.
        /// </summary>
        /// <param name="tab">The tab.</param>
        void Add(RibbonTabData tab);

        /// <summary>
        /// Adds the specified context tab.
        /// </summary>
        /// <param name="contextTab">The context tab.</param>
        void Add(RibbonContextualTabGroupData contextTab);

        /// <summary>
        /// Adds the specified button in the specified group in home menu.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="group">The group.</param>
        void AddInHome(RibbonButtonData button, Groups group);

        /// <summary>
        /// Adds the specified control into the application menu.
        /// </summary>
        /// <param name="control">The control.</param>
        void AddToApplicationMenu(RibbonControlData control);

        /// <summary>
        /// Deactivates the menu.
        /// </summary>
        void HideMainMenu();

        /// <summary>
        /// Executes the specified delegate synchronously on the thread 
        /// the System.Windows.Threading.Dispatcher is associated with.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        void Invoke(Action action);

        /// <summary>
        /// Navigates to specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        void Navigate(Page page);

        /// <summary>
        /// Navigates to last page if there's one.
        /// </summary>
        /// <param name="page">The page.</param>
        void NavigateBack();

        /// <summary>
        /// Navigates to previous page if there's one.
        /// </summary>
        /// <param name="page">The page.</param>
        void NavigateForward();

        /// <summary>
        /// Navigates to start page.
        /// </summary>
        void NavigateToStartPage();

        /// <summary>
        /// Activates the menu.
        /// </summary>
        void ShowMainMenu();

        /// <summary>
        /// Writes a message in the StatusBar.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        void WriteStatus(StatusType type, string message);

        /// <summary>
        /// Write the status "Ready." in the status box
        /// </summary>
        void WriteStatusReady();

        #endregion Methods
    }
}