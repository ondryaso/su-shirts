using System.Collections.Generic;
using SUShirts.Business.Dto;
using SUShirts.Data.Enums;

namespace SUShirts.Models
{
    public class ShirtListModel
    {
        public List<ShirtDto> Shirts { get; set; }
        public SexVariant Sex { get; set; }
    }
}
