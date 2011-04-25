using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace DataTransferObjects.ContextObjects
{
    /// <summary>
    /// Pepresents project cloning context
    /// </summary>
    [DataContract]
    [Serializable]
    [DebuggerDisplay("ProjectCloningContext: ")]
    public class ProjectCloningContext
    {
        /// <summary>
        /// Project to be cloned
        /// </summary>
        [DataMember]
        public Project Project { get; set; }

        /// <summary>
        /// Whether to clone milestones too
        /// </summary>
        [DataMember]
        public bool CloneMilestones { get; set; }

        /// <summary>
        /// Whether to Clone Billing Info too
        /// </summary>
        [DataMember]
        public bool CloneBillingInfo { get; set; }

        /// <summary>
        /// Whether to clone Billing Notes too
        /// </summary>
        [DataMember]
        public bool CloneNotes { get; set; }

        /// <summary>
        /// Whether to clone commissions too
        /// </summary>
        [DataMember]
        public bool CloneCommissions { get; set; }

        /// <summary>
        /// Project Status
        /// </summary>
        [DataMember]
        public ProjectStatus ProjectStatus { get; set; }
    }
}

