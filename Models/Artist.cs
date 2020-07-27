using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace muscshop.Models
{
    public class Artist
    {
        [Required]
        public string Name { get; set; }

        public string BasicInfo { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [NotMapped]
        public HttpPostedFileBase Photo { get; set; }

        public int ArtistId { get; set; }

        public Artist()
        {
            ImageUrl = "~/App_Files/Images/defaultartist.png";
        }
    }
}