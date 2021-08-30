using AutoMapper;
using CoreCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>()
                .ForMember(c => c.VenueName, o => o.MapFrom(c => c.Location.VenueName))
                .ForMember(c => c.Address1, o => o.MapFrom(c => c.Location.Address1))
                .ForMember(c => c.Address2, o => o.MapFrom(c => c.Location.Address2))
                .ForMember(c => c.Address3, o => o.MapFrom(c => c.Location.Address3))
                .ForMember(c => c.CityTown, o => o.MapFrom(c => c.Location.CityTown))
                .ForMember(c => c.StateProvince, o => o.MapFrom(c => c.Location.StateProvince))
                .ForMember(c => c.PostalCode, o => o.MapFrom(c => c.Location.PostalCode))
                .ForMember(c => c.Country, o => o.MapFrom(c => c.Location.Country))
                .ReverseMap();

            this.CreateMap<Talk, TalkModel>().ReverseMap()
                .ForMember(t => t.Camp, opt => opt.Ignore())
                .ForMember(t => t.Speaker, opt => opt.Ignore());

            this.CreateMap<Speaker, SpeakerModel>().ReverseMap();

        }
    }
}
