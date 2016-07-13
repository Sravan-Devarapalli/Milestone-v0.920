using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using DataTransferObjects;
using DataAccess;

namespace PracticeManagementService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "VendorService" in code, svc and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class VendorService : IVendorService
    {

        public List<Vendor> GetAllActiveVendors()
        {
            return VendorDAL.GetAllActiveVendors();
        }

        public List<VendorType> GetListOfVendorTypes()
        {
            return VendorDAL.GetAllVendorTypes();
        }

        public List<Vendor> GetListOfVendorsWithFilters(bool active, bool inactive, string vendorTypes, string looked)
        {
            return VendorDAL.GetListOfVendorsWithFilters(active, inactive, vendorTypes, looked);
        }



        public Vendor GetVendorById(int vendorId)
        {
            var vendor = VendorDAL.GetVendorById(vendorId);
            if (vendor != null && vendor.Id.HasValue)
            {
                vendor.Attachments = VendorDAL.GetVendorAttachments(vendor.Id.Value);
            }
            return vendor;
        }


        public int SaveVendorDetail(Vendor vendor, string userName)
        {
            if (vendor.Id.HasValue)
            {
                VendorDAL.UpdateVendor(vendor, userName);
            }
            else {
                VendorDAL.InsertVendor(vendor, userName);
            }

            return vendor.Id != null ? vendor.Id.Value : -1;
        }

        public void VendorValidations(Vendor vendor)
        {
            VendorDAL.VendorValidations(vendor);
        }

        public List<VendorAttachment> GetVendorAttachments(int vendorId)
        {
            return VendorDAL.GetVendorAttachments(vendorId);
        }

        public void SaveVendorAttachmentData(VendorAttachment attachment, int vendorId, string userName)
        {
            VendorDAL.SaveVendorAttachmentData(attachment, vendorId, userName);
        }

        public void DeleteVendorAttachmentById(int? attachmentId, int vendorId, string userName)
        {
            VendorDAL.DeleteVendorAttachmentById(attachmentId, vendorId, userName);
        }

        public byte[] GetVendorAttachmentData(int vendorId, int attachmentId)
        {
            return VendorDAL.GetVendorAttachmentData(vendorId, attachmentId);
        }

        public List<Project> ProjectListByVendor(int vendorId)
        {
            return VendorDAL.ProjectListByVendor(vendorId);
        }

        public List<Person> PersonListByVendor(int vendorId)
        {
            return VendorDAL.PersonListByVendor(vendorId);
        }

    }
}

