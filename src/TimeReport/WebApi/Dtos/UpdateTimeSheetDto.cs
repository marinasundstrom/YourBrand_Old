﻿namespace YourBrand.TimeReport.Dtos;

public record class UpdateTimeSheetDto(IEnumerable<UpdateEntryDto2> Entries);