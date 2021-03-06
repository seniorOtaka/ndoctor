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
namespace Probel.NDoctor.Domain.DAL.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using NHibernate;
    using NHibernate.Linq;

    using Probel.Helpers.Assertion;
    using Probel.NDoctor.Domain.DAL.Entities;
    using Probel.NDoctor.Domain.DAL.EqualityComparers;
    using Probel.NDoctor.Domain.DAL.Helpers;
    using Probel.NDoctor.Domain.DAL.Subcomponents;
    using Probel.NDoctor.Domain.DTO.Components;
    using Probel.NDoctor.Domain.DTO.Exceptions;
    using Probel.NDoctor.Domain.DTO.Objects;

    public class PatientDataComponent : BaseComponent, IPatientDataComponent
    {
        #region Constructors

        public PatientDataComponent(ISession session)
            : base(session)
        {
        }

        public PatientDataComponent()
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds the specified doctor to the specified patient.
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <param name="doctor">The doctor.</param>
        /// <exception cref="EntityNotFoundException">If there's no link between the doctor and the patient</exception>
        public void AddDoctorTo(LightPatientDto patient, LightDoctorDto doctor)
        {
            var patientEntity = this.Session.Get<Patient>(patient.Id);
            var doctorEntity = this.Session.Get<Doctor>(doctor.Id);

            new Updator(this.Session).AddDoctorTo(patientEntity, doctorEntity);
        }

        /// <summary>
        /// Adds the specified tag to the specified patient.
        /// </summary>
        /// <param name="patient">The light patient dto.</param>
        /// <param name="tag">The search tag dto.</param>
        public void AddTagTo(LightPatientDto patient, SearchTagDto tag)
        {
            var entity = (from p in this.Session.Query<Patient>()
                          where p.Id == patient.Id
                          select p).Single();
            var eTag = Mapper.Map<SearchTagDto, SearchTag>(tag);
            entity.SearchTags.Add(eTag);
            this.Session.Save(entity);
        }

        /// <summary>
        /// Binds the specified tags to the specified patient.
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <param name="tags">The tags.</param>
        public void BindTagsTo(LightPatientDto patient, IEnumerable<SearchTagDto> tags)
        {
            var ids = tags.Select(e => e.Id).ToList();
            var ePatient = this.Session.Get<Patient>(patient.Id);
            var eTags = (from t in this.Session.Query<SearchTag>()
                         where ids.Contains(t.Id)
                         select t).ToList();

            foreach (var t in eTags) { ePatient.SearchTags.Add(t); }
            this.Session.Update(ePatient);
        }

        /// <summary>
        /// Checks whether a search task exist with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        ///   <c>True</c> if a search tag exists with the specified name; otherwise <c>False</c>
        /// </returns>
        public bool CheckSearchTagExist(string name)
        {
            return new Query(this.Session).CheckSearchTagExists(name);
        }

        /// <summary>
        /// Creates the specified profession.
        /// </summary>
        /// <param name="profession">The profession.</param>
        public long Create(ProfessionDto profession)
        {
            return new Creator(this.Session).Create(profession);
        }

        /// <summary>
        /// Create the specified item into the database
        /// </summary>
        /// <param name="reputation">The item to add in the database</param>
        public long Create(ReputationDto reputation)
        {
            return new Creator(this.Session).Create(reputation);
        }

        /// <summary>
        /// Create the specified item into the database
        /// </summary>
        /// <param name="reputation">The item to add in the database</param>
        public long Create(InsuranceDto insurance)
        {
            return new Creator(this.Session).Create(insurance);
        }

        /// <summary>
        /// Create the specified item into the database
        /// </summary>
        /// <param name="reputation">The item to add in the database</param>
        public long Create(PracticeDto practice)
        {
            return new Creator(this.Session).Create(practice);
        }

        /// <summary>
        /// Create the specified item into the database
        /// </summary>
        /// <param name="doctor"></param>
        /// <returns></returns>
        public long Create(DoctorDto doctor)
        {
            return new Creator(this.Session).Create(doctor);
        }

        /// <summary>
        /// Create the specified item into the database
        /// </summary>
        /// <param name="item">The item to add in the database</param>
        /// <returns>
        /// The id of the just created item
        /// </returns>
        public long Create(TagDto item)
        {
            return new Creator(this.Session).Create(item);
        }

        /// <summary>
        /// Gets all insurances stored in the database. Return a light version of the insurance
        /// </summary>
        /// <returns>
        /// A list of light weight insurance
        /// </returns>
        public IEnumerable<LightInsuranceDto> GetAllInsurancesLight()
        {
            return new Selector(this.Session).GetAllInsurancesLight();
        }

        /// <summary>
        /// Gets all practices stored in the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LightPracticeDto> GetAllPracticesLight()
        {
            return new Selector(this.Session).GetAllPracticesLight();
        }

        /// <summary>
        /// Gets all professions stored in the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProfessionDto> GetAllProfessions()
        {
            return new Selector(this.Session).GetAllProfessions();
        }

        /// <summary>
        /// Gets all reputations stored in the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReputationDto> GetAllReputations()
        {
            return new Selector(this.Session).GetAllReputations();
        }

        /// <summary>
        /// Gets the doctor with the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The doctor with the specified id</returns>
        public DoctorDto GetDoctorById(long id)
        {
            var fullDoctor = (from p in this.Session.Query<Doctor>()
                              where p.Id == id
                              select p).FirstOrDefault();

            if (fullDoctor == null) { throw new EntityNotFoundException(typeof(Doctor)); }

            // Fix for patient created before issue 76: if address is null
            // It's impossible to update the address with the databinding.
            if (fullDoctor.Address == null) { fullDoctor.Address = new Address(); }

            return Mapper.Map<Doctor, DoctorDto>(fullDoctor);
        }

        /// <summary>
        /// Gets the doctors linked to the specified patient.
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <returns>A list of doctors</returns>
        public IEnumerable<LightDoctorDto> GetDoctorOf(LightPatientDto patient)
        {
            var entity = this.Session.Get<Patient>(patient.Id);
            if (entity.Doctors != null)
            {
                return Mapper.Map<IList<Doctor>, IList<LightDoctorDto>>(entity.Doctors);
            }
            else { return new List<LightDoctorDto>(); }
        }

        /// <summary>
        /// Gets the doctors (with full data) linked to the specified patient.
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <returns>A list of doctors</returns>
        public IEnumerable<DoctorDto> GetFullDoctorOf(LightPatientDto patient)
        {
            var entity = this.Session.Get<Patient>(patient.Id);
            if (entity.Doctors != null)
            {
                return Mapper.Map<IList<Doctor>, IList<DoctorDto>>(entity.Doctors);
            }
            else { return new List<DoctorDto>(); }
        }

        /// <summary>
        /// Gets the insurance by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// The insurance
        /// </returns>
        public InsuranceDto GetInsuranceById(long id)
        {
            if (id == 0) { return new InsuranceDto(); }
            else { return new Selector(this.Session).GetById<Insurance, InsuranceDto>(id); }
        }

        /// <summary>
        /// Gets the light patient by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public LightPatientDto GetLightPatientById(long id)
        {
            if (id == 0) { return new LightPatientDto(); }

            var result = this.Session.Get<Patient>(id);
            if (result != null) { return Mapper.Map<Patient, LightPatientDto>(result); }
            else { throw new EntityNotFoundException(typeof(Patient)); }
        }

        /// <summary>
        /// Gets all the search tags that are not binded to the specified patient
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <returns>The tags that are not assigned to the patient</returns>
        public IEnumerable<SearchTagDto> GetNotAssignedTagsOf(LightPatientDto patient)
        {
            var ids = this.Session
                .Get<Patient>(patient.Id)
                .SearchTags
                .Select(e => e.Id)
                .ToList();

            var eTags = (from t in this.Session.Query<SearchTag>()
                         where !ids.Contains(t.Id)
                         select t).ToList();
            return Mapper.Map<IEnumerable<SearchTag>, IEnumerable<SearchTagDto>>(eTags);
        }

        /// <summary>
        /// Gets the doctors that can be linked to the specified doctor.
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="on">Indicate where the search should be executed.</param>
        /// <returns>
        /// A list of doctor
        /// </returns>
        public IEnumerable<LightDoctorDto> GetNotLinkedDoctorsFor(LightPatientDto patient, string criteria, SearchOn searchOn)
        {
            var patientEntity = this.Session.Get<Patient>(patient.Id);

            List<Doctor> result = new List<Doctor>();

            if (criteria == "*") { result = this.GetAllDoctors(); }
            else
            {
                switch (searchOn)
                {
                    case SearchOn.FirstName:
                        result = this.SearchDoctorOnFirstName(criteria);
                        break;
                    case SearchOn.LastName:
                        result = this.SearchDoctorOnLastName(criteria);
                        break;
                    case SearchOn.FirstAndLastName:
                        result = this.SearchDoctorOnFirstNameAndLastName(criteria);
                        break;
                    default:
                        Assert.FailOnEnumeration(searchOn);
                        break;
                }
            }

            result = this.RemoveDoctorsOfPatient(result, patientEntity);

            return Mapper.Map<IList<Doctor>, IList<LightDoctorDto>>(result);
        }

        /// <summary>
        /// Loads all the data of the patient.
        /// </summary>
        /// <param name="patient">The patient to load.</param>
        /// <returns>A DTO with the whole data</returns>
        /// <exception cref="Probel.NDoctor.Domain.DAL.Exceptions.EntityNotFoundException">If the patient doesn't exist</exception>
        public PatientDto GetPatient(LightPatientDto patient)
        {
            return this.GetPatientById(patient.Id);
        }

        /// <summary>
        /// Loads all the data of the patient represented by the specified id.
        /// </summary>
        /// <param name="patient">The id of the patient to load.</param>
        /// <returns>A DTO with the whole data</returns>
        /// <exception cref="Probel.NDoctor.Domain.DAL.Exceptions.EntityNotFoundException">If the id is not linked to a patient</exception>
        public PatientDto GetPatientById(long id)
        {
            var fullPatient = (from p in this.Session.Query<Patient>()
                               where p.Id == id
                               select p).FirstOrDefault();

            if (fullPatient == null) { throw new EntityNotFoundException(typeof(Patient)); }

            // Fix for patient created before issue 76: if address is null
            // It's impossible to update the address with the databinding.
            if (fullPatient.Address == null) { fullPatient.Address = new Address(); }

            return Mapper.Map<Patient, PatientDto>(fullPatient);
        }

        /// <summary>
        /// Gets the practice by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public PracticeDto GetPracticeById(long id)
        {
            return new Selector(this.Session).GetById<Practice, PracticeDto>(id);
        }

        /// <summary>
        /// Gets the search tags of the specified patient.
        /// </summary>
        /// <param name="patient">The selected patient.</param>
        /// <returns>
        /// An enumeration of search tags
        /// </returns>
        public IEnumerable<SearchTagDto> GetSearchTagsOf(LightPatientDto patient)
        {
            var entity = this.Session.Get<Patient>(patient.Id);

            return Mapper.Map<IEnumerable<SearchTag>, IEnumerable<SearchTagDto>>(entity.SearchTags.ToList());
        }

        /// <summary>
        /// Gets all the tags with the specified catagory.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public IEnumerable<TagDto> GetTags(TagCategory category)
        {
            return new Selector(this.Session).GetTags(category);
        }

        /// <summary>
        /// Gets the thumbnail of the specified patient.
        /// </summary>
        /// <param name="patientDto">The patient dto.</param>
        /// <returns></returns>
        public byte[] GetThumbnail(PatientDto patientDto)
        {
            return (from p in this.Session.Query<Patient>()
                    where p.Id == patientDto.Id
                    select p.Thumbnail).Single();
        }

        /// <summary>
        /// Determines whether the specified patient has the specified doctor.
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <param name="doctor">The doctor.</param>
        /// <returns>
        ///   <c>true</c> if the specified patient has the doctor; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDoctor(LightPatientDto patient, LightDoctorDto doctor)
        {
            var entity = (from p in this.Session.Query<Patient>()
                          where p.Id == patient.Id
                          select p).Single();

            return (from d in entity.Doctors
                    where d.Id == doctor.Id
                    select d).Count() > 0;
        }

        /// <summary>
        /// Removes the link that existed between the specified patient and the specified doctor.
        /// </summary>
        /// <exception cref="EntityNotFoundException">If there's no link between the doctor and the patient</exception>
        /// <param name="patient">The patient.</param>
        /// <param name="doctor">The doctor.</param>
        public void RemoveDoctorFor(LightPatientDto patient, LightDoctorDto doctor)
        {
            new Remover(this.Session).Remove(doctor, patient);
        }

        /// <summary>
        /// Removes the specified tasks for the specified patient.
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <param name="toRemove">The tasks to remove.</param>
        public void RemoveTasksFor(LightPatientDto patient, IEnumerable<SearchTagDto> toRemove)
        {
            if (toRemove != null)
            {
                var ePatient = this.Session.Get<Patient>(patient.Id);
                var ids = toRemove.Select(e => e.Id);

                var tags = (from t in ePatient.SearchTags
                            where !ids.Contains(t.Id)
                            select t).ToList();
                ePatient.SearchTags = tags;
                this.Session.Update(ePatient);
            }
            else { return; }
        }

        /// <summary>
        /// Updates the patient with the new data.
        /// </summary>
        /// <param name="item">The patient.</param>
        public void Update(PatientDto item)
        {
            Assert.IsNotNull(item, "item");
            item.LastUpdate = DateTime.Today;

            var entity = this.Session.Get<Patient>(item.Id);

            Mapper.Map<PatientDto, Patient>(item, entity);
            if (entity.Address == null) { entity.Address = new Address(); }

            entity.Refresh(item, this.Session);

            this.Session.Merge(entity);
        }

        /// <summary>
        /// Updates the thumbnail of the specified patient.
        /// </summary>
        /// <param name="patientDto">The patient dto.</param>
        /// <param name="thumbnail">The byte array representing the thumbnail of the patient.</param>
        public void UpdateThumbnail(LightPatientDto patientDto, byte[] thumbnail)
        {
            var entity = (from p in this.Session.Query<Patient>()
                          where p.Id == patientDto.Id
                          select p).Single();
            entity.Thumbnail = thumbnail;

            this.Session.Update(entity);
        }

        private List<Doctor> GetAllDoctors()
        {
            return (from d in this.Session.Query<Doctor>()
                    select d).ToList();
        }

        private bool NotIn(Patient patient, Doctor toCheck)
        {
            return (from d in patient.Doctors
                    where d.Id == toCheck.Id
                    select d).Count() == 0;
        }

        private List<Doctor> RemoveDoctorsOfPatient(List<Doctor> result, Patient patientEntity)
        {
            return (from d in result
                    where !patientEntity.Doctors.Contains(d, new EntityEqualityComparer())
                    select d).ToList();
        }

        private List<Doctor> SearchDoctorOnFirstName(string criteria)
        {
            return (from d in this.Session.Query<Doctor>()
                    where d.FirstName.Contains(criteria)
                    select d).ToList();
        }

        private List<Doctor> SearchDoctorOnFirstNameAndLastName(string criteria)
        {
            return (from d in this.Session.Query<Doctor>()
                    where d.FirstName.Contains(criteria)
                       || d.LastName.Contains(criteria)
                    select d).ToList();
        }

        private List<Doctor> SearchDoctorOnLastName(string criteria)
        {
            return (from d in this.Session.Query<Doctor>()
                    where d.LastName.Contains(criteria)
                    select d).ToList();
        }

        #endregion Methods
    }
}