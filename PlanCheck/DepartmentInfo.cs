using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace PlanCheck
{
	public static class DepartmentInfo
	{
		/// <summary>
		/// Gets all machine IDs from a department
		/// </summary>
		/// <param name="dep"></param>
		/// <returns></returns>
		public static List<string> GetMachineIDs(Department dep)
		{
			return Departments[dep].Machines;
		}

		/// <summary>
		/// Gets all CT IDs from a department
		/// </summary>
		/// <param name="dep"></param>
		/// <returns></returns>
		public static List<string> GetCTIDs(Department dep)
		{
			return Departments[dep].CTs;
		}

		/// <summary>
		/// Gets all rad onc user names from a department
		/// </summary>
		/// <param name="dep"></param>
		/// <returns></returns>
		public static List<string> GetRadOncUserNames(Department dep)
		{
			return Departments[dep].RadOncUserNames;
		}

		/// <summary>
		/// Gets the department that the machine is in.
		/// Returns Department.None if the machine ID cannot be found
		/// </summary>
		/// <param name="machineID"></param>
		/// <returns></returns>
		public static Department GetDepartment(string machineID)
		{
			try
			{
				return Departments.Where(x => x.Value.Machines.Contains(machineID)).Select(x => x.Key).First();
			}
			catch
			{
				MessageBox.Show($"Could not find a corresponding department for machine: {machineID}\nPlease ensure that it has been added to the department list", "Unknown Machine", MessageBoxButton.OK, MessageBoxImage.Error);

				return Department.None;
			}
        }

        /// <summary>
        /// Gets the department that the machine is in.
        /// Returns Department.None if the machine ID cannot be found
        /// </summary>
        /// <param name="machineID"></param>
        /// <returns></returns>
        public static Department GetDepartment(Image image)
        {
            try
            {
				return Departments.Where(x => x.Value.CTs.Contains(CTNames.Lookup[image.Series.ImagingDeviceSerialNo])).Select(x => x.Key).First();
            }
            catch
            {
                MessageBox.Show($"Could not find a corresponding department for CT S/N: {image.Series.ImagingDeviceSerialNo}\nPlease ensure that it has been added to the department list", "Unknown Machine", MessageBoxButton.OK, MessageBoxImage.Error);

                return Department.None;
            }
        }

        /// <summary>
        /// Access to all department specific IDs
        /// </summary>
        private static Dictionary<Department, DepartmentInfoStruct> Departments = new Dictionary<Department, DepartmentInfoStruct>()
		{
			{ Department.PVH,
				new DepartmentInfoStruct {
					Machines = MachineNames.PVH,
					CTs = new List<string> { CTNames.PVH },
					RadOncUserNames = RadOncUserNames.PVH
				}
			}
		};

		/// <summary>
		/// Dictionary of machine names in Aria
		/// </summary>
		public static class MachineNames
		{
			public static readonly string PVH_1199 = "TrueBeamSTx1199";
			public static readonly string PVH_1424 = "TrueBeam1424";
			public static readonly string PVH_4960 = "TrueBeam4960";

			public static readonly List<string> PVH = new List<string> { PVH_1199, PVH_1424, PVH_4960 };
		}

		public static List<string> LinearAccelerators = new List<string>
        {
            MachineNames.PVH_1199,
            MachineNames.PVH_1424,
            MachineNames.PVH_4960
        };

		public static List<string> TrueBeams = new List<string>
		{
			MachineNames.PVH_1199,
			MachineNames.PVH_1424,
			MachineNames.PVH_4960
		};

		public static List<string> Clinacs = new List<string> { };

		public static List<string> ProtonGantries = new List<string> { };

		/// <summary>
		/// Dictionary of CT names in Aria
		/// </summary>
		private static class CTNames
		{
			public static readonly string PVH = "PVH CT Sim";

			/// <summary>
			/// Key - CT Serial Number
			/// Value - Imaging Device ID in Aria
			/// </summary>
			public static readonly Dictionary<string, string> Lookup = new Dictionary<string, string>()
			{
				{ "130126", PVH }
			};
		}

		/// <summary>
		/// Dictionary of allowable rad onc user names for plan approval
		/// </summary>
		private static class RadOncUserNames
		{
			public static readonly List<string> PVH = new List<string> { "petijo", "liuarthu", "lise", "jackmatt" }; // @"uch\petijo", @"uch\liuarthu", @"uch\lise" };
		}

		/// <summary>
		/// Dictionary of department names in Aria
		/// </summary>
		private static class DepartmentNames
		{
			public static readonly string PVH = "Poudre Valley Hospital";
		}

		/// <summary>
		/// Department specific info (machine IDs, CT IDs, Rad onc user names)
		/// </summary>
		private struct DepartmentInfoStruct
		{
			public List<string> Machines;
			public List<string> CTs;
			public List<string> RadOncUserNames;
		}
	}

	/// <summary>
	/// UCHealth Departments
	/// </summary>
	public enum Department
	{
		PVH,
		None
	}
}