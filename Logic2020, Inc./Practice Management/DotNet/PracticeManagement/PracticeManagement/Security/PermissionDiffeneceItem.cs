namespace PraticeManagement.Security
{
    public class PermissionDiffeneceItem
    {
        public string Title { get; set; }
        public bool Old { get; set; }
        public bool New { get; set; }
        public bool IsDifferent { get { return Old != New; } }
    }
}

