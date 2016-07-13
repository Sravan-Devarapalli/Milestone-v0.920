using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataTransferObjects;

namespace PracticeManagementService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IVendorService" in both code and config file together.
    [ServiceContract]
    public interface IVendorService
    {
        [OperationContract]
        List<VendorType> GetListOfVendorTypes();

        [OperationContract]
        List<Vendor> GetAllActiveVendors();

        [OperationContract]
        List<Vendor> GetListOfVendorsWithFilters(bool active, bool inactive, string vendorTypes, string looked);

        [OperationContract]
        Vendor GetVendorById(int vendorId);

        [OperationContract]
        int SaveVendorDetail(Vendor vendor, string userName);

        [OperationContract]
        void VendorValidations(Vendor vendor);

        [OperationContract]
        List<VendorAttachment> GetVendorAttachments(int vendorId);

        [OperationContract]
        void SaveVendorAttachmentData(VendorAttachment attachment, int vendorId, string userName);

        [OperationContract]
        void DeleteVendorAttachmentById(int? attachmentId, int vendorId, string userName);

        [OperationContract]
        byte[] GetVendorAttachmentData(int vendorId, int attachmentId);

        [OperationContract]
        List<Project> ProjectListByVendor(int vendorId);

        [OperationContract]
        List<Person> PersonListByVendor(int vendorId);
    }
}

