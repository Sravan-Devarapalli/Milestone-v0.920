using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataAccess.Other;
using DataTransferObjects;

namespace DataAccess
{
    /// <summary>
    /// 	Access the milestone notes data in the database.
    /// </summary>
    public static class NoteDAL
    {
        #region Methods

        /// <summary>
        /// 	Retrives a list of milestone notes for the specified milestone.
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="target"></param>
        /// <returns>A list of the <see cref = "Note" /> objects.</returns>
        public static List<Note> NoteListByTargetId(int targetId, NoteTarget target)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Note.NoteGetByTargetId, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.NoteTargetId, (int)target);
                command.Parameters.AddWithValue(Constants.ParameterNames.TargetId, targetId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                    return ReadNotes(reader);
            }
        }

        /// <summary>
        /// 	Inserts a note for the milestone into the database.
        /// </summary>
        /// <param name = "note">The data to be inserted into.</param>
        public static void NoteInsert(Note note)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Note.NoteInsert, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.TargetId, note.TargetId);
                command.Parameters.AddWithValue(Constants.ParameterNames.NoteTargetId, (int)note.Target);
                command.Parameters.AddWithValue(Constants.ParameterNames.PersonId,
                                                note.Author != null && note.Author.Id.HasValue
                                                    ? (object)note.Author.Id.Value
                                                    : DBNull.Value);
                command.Parameters.AddWithValue(Constants.ParameterNames.NoteText,
                                                !string.IsNullOrEmpty(note.NoteText)
                                                    ? (object)note.NoteText
                                                    : (object)string.Empty);
                var noteIdParameter = new SqlParameter(Constants.ParameterNames.NoteId, SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(noteIdParameter);
                connection.Open();
                command.ExecuteNonQuery();
                note.Id = (int)noteIdParameter.Value;
            }
        }

        private static List<Note> ReadNotes(DbDataReader reader)
        {
            var result = new List<Note>();

            if (reader.HasRows)
            {
                var noteIdIndex = reader.GetOrdinal(Constants.ColumnNames.NoteId);
                var targetIdIndex = reader.GetOrdinal(Constants.ColumnNames.TargetId);
                var personIdIndex = reader.GetOrdinal(Constants.ColumnNames.PersonId);
                var createDateIndex = reader.GetOrdinal(Constants.ColumnNames.CreateDate);
                var noteTextIndex = reader.GetOrdinal(Constants.ColumnNames.NoteText);
                var lastNameIndex = reader.GetOrdinal(Constants.ColumnNames.LastName);
                var firstNameIndex = reader.GetOrdinal(Constants.ColumnNames.FirstName);

                while (reader.Read())
                {
                    result.Add(
                        new Note
                            {
                                Id = reader.GetInt32(noteIdIndex),
                                TargetId = reader.GetInt32(targetIdIndex),
                                CreateDate = reader.GetDateTime(createDateIndex),
                                NoteText = reader.GetString(noteTextIndex),
                                Author = new Person
                                             {
                                                 Id = reader.GetInt32(personIdIndex),
                                                 FirstName = reader.GetString(firstNameIndex),
                                                 LastName = reader.GetString(lastNameIndex)
                                             },
                                Target = NoteTarget.Milestone
                            });
                }
            }

            return result;
        }



        public static void NoteUpdate(Note note)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Note.NoteUpdate, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.NoteId, note.Id);
                command.Parameters.AddWithValue(Constants.ParameterNames.NoteText,
                                                !string.IsNullOrEmpty(note.NoteText)
                                                    ? (object)note.NoteText
                                                    : (object)string.Empty);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void NoteDelete(int noteId)
        {
            using (var connection = new SqlConnection(DataSourceHelper.DataConnection))
            using (var command = new SqlCommand(Constants.ProcedureNames.Note.NoteDelete, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = connection.ConnectionTimeout;

                command.Parameters.AddWithValue(Constants.ParameterNames.NoteId, noteId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        #endregion
       
    }
}

