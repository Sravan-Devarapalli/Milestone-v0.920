using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using DataTransferObjects.Skills;
using DataAccess.Other;
using DataTransferObjects;


namespace DataAccess.Skills
{
    public static class PersonSkillDAL
    {
        public static List<Skill> GetSkillsAll()
        {
            var skills = new List<Skill>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.GetSkillsAll, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    ReadSkills(reader, skills);
                }
            }
            return skills;
        }

        public static List<SkillCategory> GetSkillCategoriesAll()
        {
            var skillCategories = new List<SkillCategory>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.GetSkillCategoriesAll, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    ReadSkillCategories(reader, skillCategories);
                }
            }
            return skillCategories;
        }

        public static List<SkillLevel> GetSkillLevelsAll()
        {
            var skillLevels = new List<SkillLevel>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.GetSkillLevelsAll, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    ReadSkillLevels(reader, skillLevels);
                }
            }
            return skillLevels;
        }

        public static List<SkillType> GetSkillTypesAll()
        {
            var skillTypes = new List<SkillType>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.GetSkillTypesAll, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    ReadSkillTypes(reader, skillTypes);
                }
            }
            return skillTypes;
        }

        public static List<Industry> GetIndustrySkillsAll()
        {
            var industrySkills = new List<Industry>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.GetIndustrySkillsAll, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    ReadIndustrySkills(reader, industrySkills);
                }
            }
            return industrySkills;
        }

        public static List<PersonSkill> GetPersonSkillsByPersonId(int personId)
        {
            var personSkills = new List<PersonSkill>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.GetPersonSkillsByPersonId, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    ReadPersonSkillsShort(reader, personSkills);
                }
            }
            return personSkills;
        }

        public static List<PersonIndustry> GetPersonIndustriesByPersonId(int personId)
        {
            var personIndustries = new List<PersonIndustry>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.GetPersonIndustriesByPersonId, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    ReadPersonIndustriesShort(reader, personIndustries);
                }
            }
            return personIndustries;
        }

        public static List<Person> PersonsSearchBySkills(string skillsSearchXML)
        {
            var persons = new List<Person>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.PersonsSearchBySkills, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddWithValue(Constants.ParameterNames.SkillsSearchXML, skillsSearchXML);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    ReadPersonsShort(reader, persons);
                }
            }
            return persons;

        }

        private static void ReadPersonsShort(SqlDataReader reader, List<Person> persons)
        {
            var personIdColumn = reader.GetOrdinal(Constants.ColumnNames.PersonId);
            var LastNameColumn = reader.GetOrdinal(Constants.ColumnNames.LastName);
            var firstNameColumn = reader.GetOrdinal(Constants.ColumnNames.FirstName);
            var profileIdColumn = reader.GetOrdinal(Constants.ColumnNames.ProfileId);
            var profileNameColumn = reader.GetOrdinal(Constants.ColumnNames.ProfileName);
            var profileUrlColumn = reader.GetOrdinal(Constants.ColumnNames.ProfileUrl);
            var isHighlighted = reader.GetOrdinal(Constants.ColumnNames.IsHighlighted);

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int personId = reader.GetInt32(personIdColumn);
                    Person person;
                    if (!persons.Any(p => p.Id.Value == personId))
                    {
                        person = new Person
                        {
                            Id = personId,
                            LastName = reader.GetString(LastNameColumn),
                            FirstName = reader.GetString(firstNameColumn),
                            IsHighlighted = reader.GetInt32(isHighlighted) == 1
                        };
                        person.Profiles = new List<Profile>();
                        persons.Add(person);
                    }
                    else
                    {
                        person = persons.First(p => p.Id.Value == personId);
                    }

                    if (!reader.IsDBNull(profileIdColumn))
                    {
                        Profile profile = new Profile();
                        profile.ProfileId = reader.GetInt32(profileIdColumn);
                        profile.ProfileName = reader.GetString(profileNameColumn);
                        profile.ProfileUrl = reader.GetString(profileUrlColumn);
                        person.Profiles.Add(profile);
                    }
                }
            }
        }

        private static void ReadPersonIndustriesShort(SqlDataReader reader, List<PersonIndustry> personIndustries)
        {

            var industryIdColumn = reader.GetOrdinal(Constants.ColumnNames.IndustryId);
            var yearsExperienceColumn = reader.GetOrdinal(Constants.ColumnNames.YearsExperience);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var personIndustry = new PersonIndustry
                    {
                        Industry = new Industry
                        {
                            Id = reader.GetInt32(industryIdColumn)
                        },
                        YearsExperience = reader.GetInt32(yearsExperienceColumn)
                    };
                    personIndustries.Add(personIndustry);
                }
            }
        }

        private static void ReadPersonSkillsShort(SqlDataReader reader, List<PersonSkill> personSkills)
        {
            var skillIdColumn = reader.GetOrdinal(Constants.ColumnNames.SkillId);
            var skillLevelIdColumn = reader.GetOrdinal(Constants.ColumnNames.SkillLevelId);
            var yearsExperienceColumn = reader.GetOrdinal(Constants.ColumnNames.YearsExperience);
            var lastUsedColumn = reader.GetOrdinal(Constants.ColumnNames.LastUsed);
            var skillCategoryIdColumn = reader.GetOrdinal(Constants.ColumnNames.SkillCategoryId);
            var skillTypeIdColumn = reader.GetOrdinal(Constants.ColumnNames.SkillTypeId);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var personSkill = new PersonSkill
                    {
                        Skill = new Skill
                        {
                            Id = reader.GetInt32(skillIdColumn),
                            Category = new SkillCategory
                            {
                                Id = reader.GetInt32(skillCategoryIdColumn),
                                SkillType = new SkillType
                                {
                                    Id = reader.GetInt32(skillTypeIdColumn)
                                }
                            }
                        },
                        SkillLevel = new SkillLevel
                        {
                            Id = reader.GetInt32(skillLevelIdColumn)
                        },
                        YearsExperience = reader.GetInt32(yearsExperienceColumn),
                        LastUsed = reader.GetInt32(lastUsedColumn)
                    };
                    personSkills.Add(personSkill);
                }
            }
        }

        private static void ReadSkillLevels(SqlDataReader reader, List<SkillLevel> skillLevels)
        {
            var skillLevelIdColumn = reader.GetOrdinal(Constants.ColumnNames.SkillLevelId);
            var skillLevelNameColumn = reader.GetOrdinal(Constants.ColumnNames.SkillLevelName);
            var skillLevelDefinitionColumn = reader.GetOrdinal(Constants.ColumnNames.SkillLevelDefinition);
            var displayOrderColumn = reader.GetOrdinal(Constants.ColumnNames.DisplayOrder);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var skillLevel = new SkillLevel
                    {
                        Id = reader.GetInt32(skillLevelIdColumn),
                        Description = reader.GetString(skillLevelNameColumn),
                        DisplayOrder = reader.IsDBNull(displayOrderColumn) ? null : (int?)Convert.ToInt32(reader[displayOrderColumn]),
                        Definition = reader.GetString(skillLevelDefinitionColumn)

                    };
                    skillLevels.Add(skillLevel);
                }
            }
        }

        private static void ReadSkillTypes(SqlDataReader reader, List<SkillType> skillTypes)
        {
            var skillTypeIdColumnIndex = reader.GetOrdinal(Constants.ColumnNames.SkillTypeId);
            var skillTypeDescriptionColumnIndex = reader.GetOrdinal(Constants.ColumnNames.SkillTypeDescription);
            var displayOrderColumnIndex = reader.GetOrdinal(Constants.ColumnNames.DisplayOrder);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var skillType = new SkillType
                    {
                        Id = reader.GetInt32(skillTypeIdColumnIndex),
                        Description = reader.GetString(skillTypeDescriptionColumnIndex),
                        DisplayOrder = reader.IsDBNull(displayOrderColumnIndex) ? null : (int?)Convert.ToInt32(reader[displayOrderColumnIndex]),
                    };
                    skillTypes.Add(skillType);
                }
            }
        }

        private static void ReadSkillCategories(SqlDataReader reader, List<SkillCategory> skillCategories)
        {
            var skillCategoryIdColumn = reader.GetOrdinal(Constants.ColumnNames.SkillCategoryId);
            var SkillTypeId = reader.GetOrdinal(Constants.ColumnNames.SkillTypeId);
            var skillCategoryNameColumn = reader.GetOrdinal(Constants.ColumnNames.SkillCategoryName);
            var displayOrderColumn = reader.GetOrdinal(Constants.ColumnNames.DisplayOrder);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var skillCat = new SkillCategory
                    {
                        Id = reader.GetInt32(skillCategoryIdColumn),
                        Description = reader.GetString(skillCategoryNameColumn),
                        DisplayOrder = reader.IsDBNull(displayOrderColumn) ? null : (int?)reader.GetInt32(displayOrderColumn),
                        SkillType = new SkillType
                        {
                            Id = reader.GetInt32(SkillTypeId)
                        }
                    };
                    skillCategories.Add(skillCat);
                }
            }
        }

        private static void ReadSkills(SqlDataReader reader, List<Skill> skills)
        {
            var skillCategoryIdColumn = reader.GetOrdinal(Constants.ColumnNames.SkillCategoryId);
            var skillIdColumn = reader.GetOrdinal(Constants.ColumnNames.SkillId);
            var skillNameColumn = reader.GetOrdinal(Constants.ColumnNames.SkillName);
            var displayOrderColumn = reader.GetOrdinal(Constants.ColumnNames.DisplayOrder);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var skill = new Skill
                    {
                        Id = reader.GetInt32(skillIdColumn),
                        Description = reader.GetString(skillNameColumn),
                        DisplayOrder = reader.IsDBNull(displayOrderColumn) ? null : (int?)reader.GetInt32(displayOrderColumn),
                        Category = new SkillCategory
                        {
                            Id = reader.GetInt32(skillCategoryIdColumn)
                        }
                    };
                    skills.Add(skill);
                }
            }
        }

        private static void ReadIndustrySkills(SqlDataReader reader, List<Industry> industrySkills)
        {
            var industryIdColumn = reader.GetOrdinal(Constants.ColumnNames.IndustryId);
            var industryNameColumn = reader.GetOrdinal(Constants.ColumnNames.IndustryName);
            var displayOrderColumn = reader.GetOrdinal(Constants.ColumnNames.DisplayOrder);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var industry = new Industry
                    {
                        Id = reader.GetInt32(industryIdColumn),
                        Description = reader.GetString(industryNameColumn),
                        DisplayOrder = reader.IsDBNull(displayOrderColumn) ? null : (int?)reader.GetInt32(displayOrderColumn)
                    };
                    industrySkills.Add(industry);
                }
            }
        }

        public static void SavePersonSkills(int personId, string skillsXml, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.SavePersonSkills, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.Skills, skillsXml);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLogin, userLogin);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void SavePersonIndustrySkills(int personId, string industrySkillsXml, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.SavePersonIndustrySkills, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.IndustrySkills, industrySkillsXml);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLogin, userLogin);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void SavePersonProfiles(int personId, string profilesXml, string userLogin)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.SavePersonProfiles, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.ProfilesXml, profilesXml);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLogin, userLogin);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<Profile> GetPersonProfiles(int personId)
        {
            List<Profile> result = new List<Profile>();
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.GetPersonProfiles, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);

                    connection.Open();
                    var reader = command.ExecuteReader();
                    ReadPersonProfiles(reader, result);
                }
            }
            return result;
        }

        public static void ReadPersonProfiles(SqlDataReader reader, List<Profile> profiles)
        {
            var profileIdColumn = reader.GetOrdinal(Constants.ColumnNames.ProfileId);
            var profileNameColumn = reader.GetOrdinal(Constants.ColumnNames.ProfileName);
            var profileUrlColumn = reader.GetOrdinal(Constants.ColumnNames.ProfileUrl);
            var modifiedByColumn = reader.GetOrdinal(Constants.ColumnNames.ModifiedBy);
            var modifiedByNameColumn = reader.GetOrdinal(Constants.ColumnNames.ModifiedByName);
            var modifiedDateColumn = reader.GetOrdinal(Constants.ColumnNames.ModifiedDate);
            var isDefaultColumn = reader.GetOrdinal(Constants.ColumnNames.IsDefault);

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Profile profile = new Profile();
                    profile.ProfileId = reader.GetInt32(profileIdColumn);
                    profile.ProfileName = reader.GetString(profileNameColumn);
                    profile.ProfileUrl = reader.GetString(profileUrlColumn);
                    profile.ModifiedBy = reader.GetInt32(modifiedByColumn);
                    profile.ModifiedByName = reader.GetString(modifiedByNameColumn);
                    profile.ModifiedDate = reader.GetDateTime(modifiedDateColumn);
                    profile.IsDefault = reader.GetBoolean(isDefaultColumn);
                    profiles.Add(profile);
                }
            }
        }

        public static void SavePersonPicture(int personId, byte[] pictureData, string userLogin, string pictureFileName)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.SavePersonPicture, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);
                    command.Parameters.AddWithValue(Constants.ParameterNames.UserLogin, userLogin);
                    command.Parameters.AddWithValue(Constants.ParameterNames.PictureFileName, (pictureFileName != null) ? (object)pictureFileName : DBNull.Value);
                    if (pictureData != null)
                    {
                        command.Parameters.AddWithValue(Constants.ParameterNames.PictureData, pictureData != null ? (object)pictureData : DBNull.Value);
                    }

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

        public static Person GetPersonWithHasPictureField(int personId)
        {
            Person person = null;
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (var command = new SqlCommand(Constants.ProcedureNames.GetPersonWithHasPictureField, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);

                    connection.Open();
                    var reader = command.ExecuteReader();
                    var personIdColoum = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                    var firstNameColoum = reader.GetOrdinal(Constants.ColumnNames.FirstName);
                    var lastNameColoum = reader.GetOrdinal(Constants.ColumnNames.LastName);
                    var HasPictureColoum = reader.GetOrdinal(Constants.ColumnNames.HasPicture);

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            person = new Person()
                            {
                                Id = reader.GetInt32(personIdColoum),
                                LastName = reader.GetString(lastNameColoum),
                                FirstName = reader.GetString(firstNameColoum),
                                HasPicture = reader.GetInt32(HasPictureColoum) == 1 ? true : false
                            };
                        }
                    }
                }
            }
            return person;
        }


        public static byte[] GetPersonPicture(int personId)
        {
            using (SqlConnection connection = new SqlConnection(DataSourceHelper.DataConnection))
            {
                using (SqlCommand command = new SqlCommand(Constants.ProcedureNames.GetPersonPicture, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;

                    command.Parameters.AddWithValue(Constants.ParameterNames.PersonId, personId);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    try
                    {
                        Byte[] pictureData = null;

                        if (reader.HasRows)
                        {
                            int pictureDataIndex = reader.GetOrdinal(Constants.ColumnNames.PictureData);

                            while (reader.Read())
                            {
                                pictureData = reader.IsDBNull(pictureDataIndex) ? null : (byte[])reader[pictureDataIndex];
                            }
                        }

                        return pictureData;
                    }
                    catch (Exception e)
                    {                       
                        throw e;
                    }
                }
            }

        }
    }
}

