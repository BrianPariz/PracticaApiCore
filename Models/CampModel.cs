using CoreCodeCamp.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Models
{
    public class CampModel
    {
        public int CampId { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        public string Moniker { get; set; }
        public Location Location { get; set; }
        public DateTime EventDate { get; set; } = DateTime.MinValue;
        [Range(1,100)]
        public int Length { get; set; } = 1;

        //en caso de que queramos pasarle datos de la entidad dentro de la entidad principal,
        //le ponemos la palabra de la entidad delante del atributo(en este caso Location)
        //en caso de no querer tener esta palabra delante se puede hacer un lamda profile
        public string LocationVenue { get; set; }
        public string LocationAddress1 { get; set; }
        public string LocationAddress2 { get; set; }
        public string LocationAddress3 { get; set; }
        public string LocationCityTown { get; set; }
        public string LocationStateProvince { get; set; }
        public string LocationPostalCode { get; set; }
        public string LocationCountry { get; set; }

        public ICollection<TalkModel> Talks { get; set; }
    }
}
