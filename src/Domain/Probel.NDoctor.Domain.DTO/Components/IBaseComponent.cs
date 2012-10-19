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
namespace Probel.NDoctor.Domain.DTO.Components
{
    using System;
    using System.Collections.Generic;

    using Probel.NDoctor.Domain.DTO.Objects;

    public interface IBaseComponent : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is session open.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is session open; otherwise, <c>false</c>.
        /// </value>
        bool IsSessionOpen
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Create the specified item into the database
        /// </summary>
        /// <param name="item">The item to add in the database</param>
        long Create(DrugDto item);

        /// <summary>
        /// Create the specified item into the database
        /// </summary>
        /// <param name="item">The item to add in the database</param>
        /// <returns>The id of the just created item</returns>
        long Create(PatientDto item);

        /// <summary>
        /// Create the specified item into the database
        /// </summary>
        /// <param name="item">The item to add in the database</param>
        /// <returns>The id of the just created item</returns>
        /// <returns>The id of the just created item</returns>
        long Create(LightDoctorDto item);

        /// <summary>
        /// Create the specified item into the database
        /// </summary>
        /// <param name="item">The item to add in the database</param>
        /// <returns>The id of the just created item</returns>
        long Create(TagDto item);

        /// <summary>
        /// Create the specified item into the database
        /// </summary>
        /// <param name="item">The item to add in the database</param>
        /// <returns>The id of the just created item</returns>
        long Create(UserDto item);

        /// <summary>
        /// Creates a new patient.
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <returns>The id of the just created item</returns>
        long Create(LightPatientDto patient);

        /// <summary>
        /// Gets the doctors by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="on">The on.</param>
        /// <returns></returns>
        IList<LightDoctorDto> GetDoctorsByNameLight(string name, SearchOn on);

        /// <summary>
        /// Gets the doctors by specialisation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="on">The on.</param>
        /// <returns></returns>
        IList<LightDoctorDto> GetDoctorsBySpecialisationLight(TagDto specialisation);

        /// <summary>
        /// Gets the drugs which has in their name the specified criteria.
        /// </summary>
        /// <param name="name">The criteria.</param>
        /// <returns>A list of drugs</returns>
        IList<DrugDto> GetDrugsByName(string name);

        /// <summary>
        /// Gets the drugs by tags.
        /// </summary>
        /// <param name="tag">The tag name.</param>
        /// <returns>A list of drugs</returns>
        IList<DrugDto> GetDrugsByTags(string tagName);

        /// <summary>
        /// Gets all insurances that contain the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IList<InsuranceDto> GetInsurances(string name);

        /// <summary>
        /// Gets the light doctor by specialisation.
        /// </summary>
        /// <param name="specialisation">The tag.</param>
        /// <returns></returns>
        IList<LightDoctorDto> GetLightDoctor(TagDto specialisation);

        /// <summary>
        /// Gets all pathologies that contains the specified name.
        /// </summary>
        /// <returns></returns>
        IList<PathologyDto> GetPathology(string name);

        /// <summary>
        /// Gets the patients that fullfill the specified criterium.
        /// </summary>
        /// <param name="criterium">The criterium.</param>
        /// <param name="search">The search should be done on the specified property.</param>
        /// <returns></returns>
        IList<LightPatientDto> GetPatientsByNameLight(string criterium, SearchOn search);

        /// <summary>
        /// Gets all practices that contains the specified name.
        /// </summary>
        /// <returns></returns>
        IList<PracticeDto> GetPractices(string name);

        /// <summary>
        /// Gets all professions that contains the specified name.
        /// </summary>
        /// <returns></returns>
        IList<ProfessionDto> GetProfessions(string name);

        /// <summary>
        /// Gets all reputations that contains the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IList<ReputationDto> GetReputations(string name);

        /// <summary>
        /// Gets all the tags with the specified catagory.
        /// </summary>
        /// <returns></returns>
        IList<TagDto> GetTags(TagCategory category);

        /// <summary>
        /// Gets the tag for patient that contain the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IList<TagDto> GetTags(string name, TagCategory type);

        /// <summary>
        /// Gets the user by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        UserDto GetUserById(long id);

        /// <summary>
        /// Gets all doctors.
        /// </summary>
        /// <returns>The light weight version of the doctors</returns>
        IList<LightDoctorDto> GetAllDoctorsLight();

        /// <summary>
        /// Gets all drugs from the database.
        /// </summary>
        /// <returns></returns>
        IList<DrugDto> GetAllDrugs();

        /// <summary>
        /// Gets all insurances stored in the database.
        /// </summary>
        /// <returns></returns>
        IList<InsuranceDto> GetAllInsurances();

        /// <summary>
        /// Gets all insurances stored in the database. Return a light version of the insurance
        /// </summary>
        /// <returns>A list of light weight insurance</returns>
        IList<LightInsuranceDto> GetAllInsurancesLight();

        /// <summary>
        /// Gets all pathologies stored in the database.
        /// </summary>
        /// <returns></returns>
        IList<PathologyDto> GetAllPathologies();

        /// <summary>
        /// Gets all the patients.
        /// </summary>
        /// <returns></returns>
        IList<LightPatientDto> GetAllPatientsLight();

        /// <summary>
        /// Gets all practices stored in the database.
        /// </summary>
        /// <returns></returns>
        IList<PracticeDto> GetAllPractices();

        /// <summary>
        /// Gets all practices stored in the database.
        /// </summary>
        /// <returns></returns>
        IList<LightPracticeDto> GetAllPracticesLight();

        /// <summary>
        /// Gets all professions stored in the database.
        /// </summary>
        /// <returns></returns>
        IList<ProfessionDto> GetAllProfessions();

        /// <summary>
        /// Gets all reputations stored in the database.
        /// </summary>
        /// <returns></returns>
        IList<ReputationDto> GetAllReputations();

        /// <summary>
        /// Gets all roles light.
        /// </summary>
        /// <returns>An array with all the roles</returns>
        IList<RoleDto> GetAllRolesLight();

        /// <summary>
        /// Gets all the tags
        /// </summary>
        /// <returns></returns>
        IList<TagDto> GetAllTags();

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns></returns>
        IList<LightUserDto> GetAllUsers();

        /// <summary>
        /// Removes item with the specified id.
        /// </summary>
        /// <typeparam name="T">The type of the item to remove</typeparam>
        /// <param name="id">The id of the item to remove.</param>
        void Remove(LightDoctorDto item);

        /// <summary>
        /// Removas the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Remove(LightPatientDto item);

        /// <summary>
        /// Removas the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Remove(PatientDto item);

        /// <summary>
        /// Updates the patient with the new data.
        /// </summary>
        /// <param name="item">The patient.</param>
        void Update(PatientDto item);

        /// <summary>
        /// Updates the specified picture.
        /// </summary>
        /// <param name="picture">The picture.</param>
        void Update(PictureDto item);

        #endregion Methods
    }
}