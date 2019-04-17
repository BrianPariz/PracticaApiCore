using AutoMapper;
using CoreCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            //usar solo ForMember si no queremos poner el nombre de la entidad dentro de otra, para traer sus datos en un modelo
            this.CreateMap<Camp, CampModel>();
                //.ForMember(c => c.Venue, o => o.MapFrom(m => m.Location.VenueName));
        }
    }
}