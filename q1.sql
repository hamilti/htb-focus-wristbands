commit;

update htb_bands.band_prints set band_print_date = 0
where guestid in (
3308, 4635, 2363, 5221,2911, 5228, 4631,
2759, 2760, 2762, 4982, 4917, 5238, 4032,
4943, 4554, 2314, 5230, 4550, 4979, 4107,
3275, 5265, 1107, 4777, 5227, 1909, 1908,
5264, 4197, 4198, 2761, 2309, 4430,
5195, 5255, 5288, 3167
);

select * from htb_focus.focus_guests
where lastname like '%yung%';


insert into band_prints (guestid, band_print_date)
select guestid, band_print_date from htb_focus.focus_guests;

