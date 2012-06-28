
CREATE or replace VIEW view_wristbands 
AS select g.title AS title,
g.firstName AS firstName,g.lastName AS lastName,g.guestID AS guestID,
bg.bookingID AS bookingID,g.gender AS gender,g.dateOfBirth AS dateOfBirth,
g.phone AS phone,g.email AS email,g.churchID AS churchID,g.age AS age,
g.firstFocus AS firstFocus,g.schoolYear AS schoolYear,g.band_streamID AS band_streamID,
hbbp.band_print_date AS band_print_date,
left((case when (c.churchID <> 0) then c.churchDesc else g.churchOther end),40) AS churchDesc,
(case when (g.band_streamID in (13,14)) then (select ucase(group_concat(distinct (case a.accommodationDesc 
when 'Full week pass' then a.accommodationDesc else left(a.accommodationDesc,3) end) separator ', ')) 
from (focus_booking_accommodation ba left join focus_accommodation a 
on((ba.accommodationID = a.accommodationID))) 
where (ba.bookingID = bg.bookingID)) 
else NULL end) AS days,
    (select max(inner_g.phone) 
    from (focus_guests inner_g 
    join focus_booking_groups inner_bg on((inner_g.guestID = inner_bg.guestID))) 
    where ((inner_bg.bookingID = bg.bookingID) and (soundex(inner_g.lastName) = soundex(g.lastName))) 
    group by bg.bookingID) AS family_phone,
    (select max(inner_g.phone) from (focus_guests inner_g 
    join focus_booking_groups inner_bg on((inner_g.guestID = inner_bg.guestID))) 
    where (inner_bg.bookingID = bg.bookingID) 
    group by bg.bookingID) AS booking_phone,s.streamDesc AS streamDesc,s.streamColour 
    AS streamColour,st.statusDesc AS statusDesc,(case g.band_streamID when 13 then 1 when 14 then 1 else 0 end) 
    AS offsite, b.statusID AS statusID 
from ((((((focus_guests g 
left join htb_bands.band_prints hbbp on((g.guestID = hbbp.guestID))) 
join focus_booking_groups bg on((g.guestID = bg.guestID))) 
join focus_band_streams s on((g.band_streamID = s.streamID))) 
join focus_bookings b on((bg.bookingID = b.bookingID))) 
join focus_status st on((b.statusID = st.statusID))) 
left join focus_churches c on((g.churchID = c.churchID))) 
where (g.active = 1) order by g.lastName;

