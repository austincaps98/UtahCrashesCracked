using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UtahCrashesCracked.Models
{
    public class Crash
    {
        [Key]
        [Required(ErrorMessage ="Please enter a valid Crash ID")]
        public int crash_id { get; set; }
        [Required(ErrorMessage ="Please choose a valid date")]
        public DateTime crash_datetime { get; set; }
        [MaxLength(10, ErrorMessage = "Route name can't be more than 10 characters.")]
        public string route { get; set; }
        [MaxLength(7, ErrorMessage = "Please enter a valid Milepoint")]
        public string milepoint { get; set; }
        [MaxLength(13, ErrorMessage = "Please enter a valid longitude")]
        public string lat_utm_y { get; set; }
        [MaxLength(13, ErrorMessage = "Please enter a valid latitude")]
        public string long_utm_x { get; set; }
        [MaxLength(50, ErrorMessage = "Road name can't be more than 50 characters.")]
        public string main_road_name { get; set; }
        [MaxLength(25, ErrorMessage = "City name can't be more than 25 characters.")]
        public string city { get; set; }
        [MaxLength(25, ErrorMessage = "County name can't be more than 25 characters.")]
        public string county_name { get; set; }
        public int crash_severity_id { get; set; }
        public string work_zone_related { get; set; }
        public string pedestrian_involved { get; set; }
        public string bicyclist_involved { get; set; }
        public string motorcycle_involved { get; set; }
        public string improper_restraint { get; set; }
        public string unrestrained { get; set; }
        public string dui { get; set; }
        public string intersection_related { get; set; }
        public string wild_animal_related { get; set; }
        public string domestic_animal_related { get; set; }
        public string overturn_rollover { get; set; }
        public string commercial_motor_veh_involved { get; set; }
        public string teenage_driver_involved { get; set; }
        public string older_driver_involved { get; set; }
        public string night_dark_condition { get; set; }
        public string single_vehicle { get; set; }
        public string distracted_driving { get; set; }
        public string drowsy_driving { get; set; }
        public string roadway_departure { get; set; }
    }
}
