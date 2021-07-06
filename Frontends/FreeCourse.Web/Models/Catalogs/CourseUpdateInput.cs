using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Models.Catalogs
{
    public class CourseUpdateInput
    {
        public string Id { get; set; }

        [Display(Name = "Kurs Adı")]
        public string Name { get; set; }

        [Display(Name = "Kurs Açıklama")]
        public string Description { get; set; }

        [Display(Name = "Kurs Fiyat")]
        public decimal Price { get; set; }

        [Display(Name = "Kurs Resim")]
        public string Picture { get; set; }
        public string UserId { get; set; }
        public FeatureViewModel Feature { get; set; }

        [Display(Name = "Kurs Kategori")]
        public string CategoryId { get; set; }
    }
}
