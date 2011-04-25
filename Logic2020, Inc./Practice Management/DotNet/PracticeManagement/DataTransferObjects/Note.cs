using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents a note for an entity.
	/// </summary>
	[Serializable]
	[DataContract]
	public class Note
	{
		/// <summary>
		/// Gets or sets an ID of the note.
		/// </summary>
		[DataMember]
		public int? Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an ID of the entity the note was written for.
		/// </summary>
		[DataMember]
		public int TargetId
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets of an target of the note.
        /// </summary>
        [DataMember]
        public NoteTarget Target
        {
            get;
            set;
        }

		/// <summary>
		/// Gets or sets of an author of the note.
		/// </summary>
		[DataMember]
		public Person Author
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets of a date when the note was created.
		/// </summary>
		[DataMember]
		public DateTime CreateDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a text of the note.
		/// </summary>
		[DataMember]
		public string NoteText
		{
			get;
			set;
		}
	}
}

