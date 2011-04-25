using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DataTransferObjects
{
    /// <summary>
    /// Enumeration for permission targets.
    /// Corresponds to the database tables.
    /// </summary>
    public enum PermissionTarget
    {
        Client  = 1,
        Group   = 2,
        Salesperson = 3,
        PracticeManager = 4,
        Practice = 5
    }

    /// <summary>
    /// Keeps track on user's permissions.
    /// 
    /// Represented by corresponding between permission target 
    ///     and list of ID's that user is ALLOWED to see.
    ///     
    /// NULL value for the list means that user is allowed to see all items in the list
    /// </summary>
    [DataContract]
    [Serializable]
    public class PersonPermission
    {
        [DataMember]
        private Dictionary<PermissionTarget, List<int>> permissions;

        public PersonPermission()
        {
            permissions = new Dictionary<PermissionTarget, List<int>>();

            //  Make sure every possible key value is taken.
            //  This actually means that by default with such values
            //      user is not allow to see anything
            foreach (PermissionTarget pt in Enum.GetValues(typeof(PermissionTarget)))
                permissions.Add(pt, new List<int>());
        }

        public void AddToList(PermissionTarget pt, int value)
        {
            if (permissions[pt] == null)
                permissions[pt] = new List<int>();

            permissions[pt].Add(value);
        }

        public void AllowAll(PermissionTarget pt)
        {
            //  Null value means everything is allowed
            permissions[pt] = null;
        }

        public List<int> GetPermissions(PermissionTarget pt)
        {
            return permissions[pt];
        }

        public void SetPermissions(PermissionTarget pt, IEnumerable<int> newPerm)
        {
            if (newPerm == null)
                permissions[pt] = null;
            else
                permissions[pt] = new List<int>(newPerm);
        }


        /// <summary>
        /// Checks if user is not allowed to see any child control
        /// </summary>
        public bool IsAllowedNothing()
        {
            bool res = true;

            foreach (PermissionTarget pt in permissions.Keys)
                if (!IsAllowedNothing(pt))
                    return false;

            return res;
        }

        /// <summary>
        /// Checks if user is not allowed to see specific control
        /// </summary>
        /// <param name="pt">Permission target</param>
        public bool IsAllowedNothing(PermissionTarget pt)
        {
            return (permissions[pt] != null && permissions[pt].Count == 0);
        }

        /// <summary>
        /// Shows permissions as string list
        /// </summary>
        /// <param name="pt">Permission to look for</param>
        /// <returns>Commaseparated list of values</returns>
        public string GetPermissionsAsStringList(PermissionTarget pt)
        {
            if (GetPermissions(pt) != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (int id in GetPermissions(pt))
                    sb.Append(id).Append(',');

                return sb.ToString();
            }

            return null;
        }

        private static T NumToEnum<T>(int number)
        {
            return (T)Enum.ToObject(typeof(T), number);
        }

        /// <summary>
        /// Converts integet into enum instance
        /// </summary>
        /// <param name="val">Integer value</param>
        /// <returns>Enum value</returns>
        public static PermissionTarget ToEnum(int val)
        {
            return NumToEnum<PermissionTarget>(val);
        }

        /// <summary>
        /// Converts object into enum instance
        /// </summary>
        /// <param name="val">Object value</param>
        /// <returns>Enum value</returns>
        public static PermissionTarget ToEnum(object obj)
        {
            return (PermissionTarget)Enum.ToObject(typeof(PermissionTarget), obj);
        }


        public override string ToString()
        {
            var sb = new StringBuilder("Permissions: \n");

            foreach (var permission in permissions)
            {
                sb.AppendFormat("Target {0} IDs: ", permission.Key);

                if (permission.Value == null)
                    sb.AppendFormat("ALL ");
                else
                    foreach (var id in permission.Value)
                        sb.AppendFormat("{0} ", id);

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}

