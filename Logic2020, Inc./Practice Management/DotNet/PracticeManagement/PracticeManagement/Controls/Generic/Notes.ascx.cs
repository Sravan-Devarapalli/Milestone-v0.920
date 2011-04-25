using System;
using System.Web.UI.WebControls;
using DataTransferObjects;

namespace PraticeManagement.Controls.Generic
{
    public partial class Notes : PracticeManagementUserControl
    {
        #region Constants

        private const string ViewStateTarget = "Target";
        private const string ViewStateGridVisible = "GridVisible";
        private const string ParameterNoteTargetId = "noteTargetId";

        #endregion

        #region Events

        public event EventHandler NoteAdded;

        #endregion


        #region Properties

        public NoteTarget Target
        {
            get { return GetViewStateValue(ViewStateTarget, NoteTarget.Milestone); }
            set { SetViewStateValue(ViewStateTarget, value); }
        }

        public bool GridVisible
        {
            get { return GetViewStateValue(ViewStateGridVisible, true); }
            set
            {
                SetViewStateValue(ViewStateGridVisible, value);
                gvNotes.Visible = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            odsNotes.SelectParameters[ParameterNoteTargetId].DefaultValue = ((int)Target).ToString();
        }

        protected void btnAddNote_Click(object sender, EventArgs e)
        {
            Page.Validate(tbNote.ValidationGroup);

            if (Page.IsValid)
            {
                var note = new Note
                               {
                                   Author = new Person
                                                {
                                                    Id = DataHelper.CurrentPerson.Id
                                                },
                                   CreateDate = DateTime.Now,
                                   NoteText = tbNote.Text,
                                   Target = Target,
                                   TargetId = Page.SelectedId.Value
                               };

                ServiceCallers.Custom.Milestone(client => client.NoteInsert(note));

                gvNotes.DataBind();

                tbNote.Text = string.Empty;

                Utils.Generic.InvokeEventHandler(NoteAdded, this, e);
            }
        }

        protected void cvLen_OnServerValidate(object source, ServerValidateEventArgs args)
        {
            var length = tbNote.Text.Length;
            args.IsValid = length > 0 && length <= 2000;
        }
    }
}
