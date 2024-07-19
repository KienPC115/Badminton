﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Badminton.Data.Models;
using Badminton.Business;
using Microsoft.AspNetCore.Mvc.Rendering;
using Badminton.Business.Shared;
using Microsoft.Extensions.Options;

namespace Badminton.RazorWebApp.Pages.CourtPage {
    public class IndexModel : CustomPage {
        private readonly ICourtBusiness _courtBusiness;
        private readonly IConfiguration _configuration;
        private readonly CourtConfiguration _courtConfiguration;

        public IndexModel(IConfiguration configuration) {
            _courtBusiness ??= new CourtBusiness();
            _configuration = configuration;
            _courtConfiguration = new CourtConfiguration();
            configuration.GetSection(nameof(CourtConfiguration)).Bind(_courtConfiguration);
        }
        // Sorting
        public string NameSort { get; set; }
        public string PriceSort { get; set; }
        public string CreatedDateSort { get; set; }
        public string UpdatedDateSort { get; set; }
        // Pagination 
        public int PageSize { get; set; }
        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; set; }
        // Filter
        [BindProperty(SupportsGet = true)]
        public string SelectedYardType { get; set; } = "All";
        public List<SelectListItem> YardType { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SelectedType { get; set; } = "All";
        public List<SelectListItem> Type { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SelectedStatus { get; set; } = "All";
        public List<SelectListItem> Status { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SelectedLocation { get; set; } = "All";
        public List<SelectListItem> Location { get; set; }

        /*public List<SelectListItem> YardType { get; set; } = CourtShared.YardType().Select(s => new SelectListItem { Value = s, Text = s }).ToList();*/
        // Current
        public string CurrentSort { get; set; }
        public string CurrentSearch { get; set; }

        public IList<Court> Court { get; set; } = default!;

        public async Task OnGetAsync(string sortOrder, string search = "", int pageNumber = 1) {
            IsAdmin = CheckAdmin();

            PageSize = int.TryParse(_configuration["PageSize"], out int ps) ? ps : 3;
            YardType = _courtConfiguration.YardType.Select(s => new SelectListItem { Value = s, Text = s }).ToList();
            Type = _courtConfiguration.Type.Select(s => new SelectListItem { Value = s, Text = s }).ToList();
            Status = _courtConfiguration.Status.Select(s => new SelectListItem { Value = s, Text = s }).ToList();
            Location = _courtConfiguration.Location.Select(s => new SelectListItem { Value = s, Text = s }).ToList();

            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            PriceSort = sortOrder == "Price" ? "price_desc" : "Price";
            CreatedDateSort = sortOrder == "CreatedTime" ? "create_desc" : "CreatedTime";
            UpdatedDateSort = sortOrder == "UpdatedTime" ? "update_desc" : "UpdatedTime";

            CurrentSort = sortOrder;
            CurrentSearch = search;

            var courtsResult = await _courtBusiness.GetCourtsWithCondition(search, sortOrder, SelectedType, SelectedYardType, SelectedStatus, SelectedLocation);
            
            if (courtsResult.Status <= 0) {
                TempData["message"] = courtsResult.Message;
                return;
            }

            var list = courtsResult.Data as IList<Court>;

            TotalPages = (int)Math.Ceiling(list.Count / (double)PageSize);
            PageNumber = pageNumber;
            Court = list.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToList();
        }
    }
}
