using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using DataTransferObjects.Skills;
using DataAccess.Other;


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
            var displayOrderColumn = reader.GetOrdinal(Constants.ColumnNames.DisplayOrder);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var skillLevel = new SkillLevel
                    {
                        Id = reader.GetInt32(skillLevelIdColumn),
                        Description = reader.GetString(skillLevelNameColumn),
                        DisplayOrder = reader.IsDBNull(displayOrderColumn) ? null : (int?)reader.GetInt32(displayOrderColumn)
                    };
                    skillLevels.Add(skillLevel);
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
    }
}

