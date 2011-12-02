﻿#region Header

/*
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

#endregion Header

namespace Probel.NDoctor.Plugins.DbConvert.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;

    using Probel.Helpers.Events;
    using Probel.NDoctor.Domain.DTO.Components;
    using Probel.NDoctor.Domain.DTO.Objects;
    using Probel.NDoctor.Plugins.DbConvert.Properties;

    public class ImportEngine
    {
        #region Fields

        private string path;

        #endregion Fields

        #region Constructors

        public ImportEngine(string path)
        {
            this.path = path;
        }

        #endregion Constructors

        #region Events

        public event EventHandler<EventArgs<string>> Logged;

        public event EventHandler<EventArgs<int>> ProgressChanged;

        #endregion Events

        #region Methods

        public bool Check()
        {
            var connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", this.path));
            var hasSucceeded = false;
            try
            {
                connection.Open();
                hasSucceeded = true;
                connection.Close();
            }
            catch (Exception ex)
            {
                this.OnLogged(ex.ToString());
                hasSucceeded = false;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed) connection.Close();
            }
            return hasSucceeded;
        }

        public void Import(IImportComponent component)
        {
            var connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", this.path));
            IEnumerable<PatientDto> result = new List<PatientDto>();
            try
            {
                this.OnLogged(Messages.Log_StartImporting);

                var patients = new PatientImporter(connection, component);
                patients.Logged += (sender, e) => this.OnLogged(e.Data);

                patients.Import();

                this.OnLogged(Messages.Log_EndImporting);
                this.OnProgressChanged(100);
            }
            catch (Exception ex)
            {
                this.OnLogged(ex.ToString());
            }
            finally
            {
                if (connection.State != ConnectionState.Closed) connection.Close();
            }
        }

        private void OnLogged(string log)
        {
            if (this.Logged != null)
                this.Logged(this, new EventArgs<string>(log));
        }

        private void OnProgressChanged(int percentage)
        {
            if (percentage < 0) percentage = 0;
            else if (percentage > 100) percentage = 100;

            if (this.ProgressChanged != null)
                this.ProgressChanged(this, new EventArgs<int>(percentage));
        }

        #endregion Methods
    }
}