SELECT  * FROM focus_guests g
inner join focus_booking_groups bg on (g.guestID = bg.guestID)
inner join focus_bookings b on (bg.bookingID = b.bookingID)
left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID  )
left outer join focus_accommodation a on (ba.accommodationID = a.accommodationID)
where bg.bookingID in (2915)
and b.statusID not in (0, 4, 7, 8, 6)
and g.active = 1
order by bg.bookingID asc, guesttypeid desc;

SELECT bg.bookingID, 
group_concat(DISTINCT a.accommodationDesc SEPARATOR ', ') as days
FROM focus_guests g
inner join focus_booking_groups bg on (g.guestID = bg.guestID)
left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID  )
left outer join focus_accommodation a on (ba.accommodationID = a.accommodationID)
where bg.bookingID in (2944, 2392, 1326)
group by bg.bookingID;

select count(*), inner_1.guestid from (
SELECT g.guestid FROM focus_guests g
inner join focus_booking_groups bg on (g.guestID = bg.guestID)
left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID  )
left outer join focus_accommodation a on (ba.accommodationID = a.accommodationID)
where g.guestid <= 4183
order by bg.bookingID asc, guesttypeid desc
) inner_1 
group by guestid having count(*) > 1;

update focus_guests
set letter_stage = 0;

commit;


SELECT *, 
concat(firstname, ' ', lastname) as full_name, concat(title,' ', firstname, ' ', lastname) as address_name,
date_format(now(), '%D %M %Y') as curr_date,
 CASE b.statusID WHEN 0 THEN 'Invalid'
      WHEN 1 THEN 'Deposit Received, Awaiting Approval (the Focus office will be in touch soon with the next step in the process)'
      WHEN 2 THEN 'Booking Approved, Select Accommodation (please select your accommodation so that you can complete your booking)'
      WHEN 4 THEN 'Cancelled'
      WHEN 5 THEN 'Booking Received, In Pastoral Check (we are currently awaiting feedback from your church and will in touch soon with the next step in the process)'
      WHEN 6 THEN 'Booking Received, and sent back to guest'
      WHEN 9 THEN 'Booking Received, Awaiting Church Approval (we are currently awaiting feedback from your church and will in touch soon with the next step in the process)'
      WHEN 10 THEN 'Booking Received, In Church Approval (we are currently awaiting feedback from your church and will in touch soon with the next step in the process)'
      WHEN 11 THEN 'Terms and Conditions Accepted, Awaiting Payment (please pay your deposit so that you can process your booking and you can select your accommodation)'
      WHEN 12 THEN 'Accommodation Selected, Finalise Booking (please finalise your accommodation so that you can complete your booking)'
      WHEN 13 THEN 'Booking Finalised, Awaiting Final Payment (please make you final payment so that your booking can be completed)'
      WHEN 14 THEN 'Final Payment Received, Awaiting Other Guests (please encourage your other guests to make their final payment so that  your booking can be completed)'
      WHEN 15 THEN 'Booking Complete and Confirmed'
          ELSE 'Invalid'
       END AS status_message
FROM focus_accommodation a          
INNER JOIN focus_accommodation_types AS t ON (a.accommodationTypeID = t.accommodationTypeID)
INNER JOIN focus_accommodation_kinds AS k ON (a.accommodationKindID = k.accommodationKindID)
INNER JOIN focus_booking_accommodation AS ba ON a.accommodationID = ba.accommodationID
INNER JOIN focus_inhabiting AS i ON (ba.inhabitingID = i.inhabitingID)
inner join focus_bookings b on (b.bookingID = i.bookingID)
inner join focus_booking_groups bg on (b.bookingID = bg.bookingID)
inner join focus_guests g on (bg.guestID = g.guestID)
left outer join focus_churches c on (g.churchID = c.churchID)
where g.active = 1
and staff = 1
and letter_stage = 1
and ba.accommodationID not in (13,14,15,16,16,18,19)
order by b.bookingID asc, guesttypeid desc
;



#all non staff - 9/6/2011 - no day guests
SELECT * FROM focus_guests g
inner join focus_booking_groups bg on (g.guestID = bg.guestID)
inner join focus_bookings b on (bg.bookingID = b.bookingID)
left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID  )
left outer join focus_accommodation a on (ba.accommodationID = a.accommodationID)
where g.guestid <= 4183 and ba.accommodationID not in (13,14,15,16,16,18,19)
and b.statusID not in (0, 4, 7, 8, 11, 6)
and g.staff = 0
order by bg.bookingID asc, guesttypeid desc;

#all staff - 9/6/2011 - no day guests
SELECT * FROM focus_guests g
inner join focus_booking_groups bg on (g.guestID = bg.guestID)
inner join focus_bookings b on (bg.bookingID = b.bookingID)
left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID)
left outer join focus_accommodation a on (ba.accommodationID = a.accommodationID)
left outer join focus_churches c on (g.churchID = c.churchID)
where ba.accommodationID not in (13,14,15,16,16,18,19)
and b.statusID not in (0, 4, 7, 8, 11, 6)
and g.staff = 1
order by bg.bookingID asc, guesttypeid desc;


SELECT * FROM focus_guests g
inner join focus_booking_groups bg on (g.guestID = bg.guestID)
inner join focus_bookings b on (bg.bookingID = b.bookingID)
left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID  )
left outer join focus_accommodation a on (ba.accommodationID = a.accommodationID)
left outer join focus_churches c on (g.churchID = c.churchID)
where staff = 1
and letter_stage = 1
order by bg.bookingID asc, guesttypeid desc;

select date_format(now(), '%D %M %Y')
from focus_guests g
     inner join focus_booking_groups bg on (g.guestID = bg.guestID)
     left outer join focus_churches c on (g.churchID = c.churchID)
where staff = 1
and letter_stage = 1
;

select * from focus_booking_groups;

select * from focus_guests order by guestid asc;

select max(guestid) from focus_guests;

select * from focus_guests where band_streamID = 13;

select * from htb_focus.focus_streams order by streamID;

select * from htb_focus.focus_status order by statusID;

select * from focus_bookings;

select * from focus_guest_types;

select * from view_wristbands where band_streamID = 12;



#and statusID not in (0, 4, 6, 11) 
and guestID < 4974
and guestID = 2977
order by guestID;

select * from view_offsite_wristbands;

select count(*) from focus_guests g
inner join focus_booking_groups bg on (g.guestID = bg.guestID)
inner join focus_bookings b on (bg.bookingID = b.bookingID)
where band_streamID = 11 and active = 1
and statusID not in (0, 4, 6, 11) and g.guestID < 4974;


grant all on *.* to 'htbadmin'@'192.168.%';

#--------------------------------------------------
#changes to guests table

alter table focus_guests add (staff integer default 0);

update focus_guests upd 
set staff = (SELECT CASE min(c.couponID)
          WHEN 1 THEN 1
          WHEN 2 THEN 1
          WHEN 3 THEN 1
          ELSE 0
       END AS STAFF
FROM focus_booking_groups bg
     left outer join focus_guest_coupons gc on (bg.guestID = gc.guestID)
     left outer join focus_coupons c on (gc.couponID = c.couponID)
where bg.bookingID = (select bookingID FROM focus_booking_groups bg where bg.guestID = upd.guestID));

commit;

#letter_stage 0 = not printed; 1 = printing; 2 = printed;

alter table focus_guests add (letter_stage integer default 0);



# set state to printing
-- update focus_guests as target
-- set target.letter_stage = 1
-- where target.guestID in (
-- SELECT guestID from focus_booking_groups bg 
-- inner join focus_bookings b on (bg.bookingID = b.bookingID)
-- left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID  )
-- left outer join focus_accommodation a on (ba.accommodationID = a.accommodationID)
-- where b.statusID not in (0, 4, 7, 8, 6)) 
-- and target.letter_stage = 0;

# set state to printing for all staff
-- update focus_guests as target
-- set target.letter_stage = 1
-- where target.guestID in (
-- SELECT guestID from focus_booking_groups bg 
-- inner join focus_bookings b on (bg.bookingID = b.bookingID)
-- left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID  )
-- left outer join focus_accommodation a on (ba.accommodationID = a.accommodationID)
-- where ba.accommodationID not in (13,14,15,16,16,18,19)
-- and b.statusID not in (0, 4, 7, 8, 6)) 
-- and target.staff = 1
-- and target.letter_stage = 0;

# set state to printing for all non staff - type day pass
-- update focus_guests as target
-- set target.letter_stage = 1
-- where target.guestID in (
-- SELECT guestID from focus_booking_groups bg 
-- inner join focus_bookings b on (bg.bookingID = b.bookingID)
-- left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID  )
-- left outer join focus_accommodation a on (ba.accommodationID = a.accommodationID)
-- where ba.accommodationID in (13,14,15,16,16,18,19)
-- and b.statusID not in (0, 4, 7, 8, 6)) 
-- and target.staff = 0
-- and target.letter_stage = 0;

#setup all non letter sent types to send letter
update focus_guests as target
set target.letter_stage = 2
where target.guestID in (
SELECT guestID from focus_booking_groups bg 
inner join focus_bookings b on (bg.bookingID = b.bookingID)
where b.statusID not in (0, 4, 7, 8, 6)) 
and target.letter_stage = 0
and target.active = 1;

commit;

#after printing reset
update focus_guests
set letter_stage = 2
where letter_stage = 1;

# Setup wristband printing

alter table focus_guests add (band_print_date integer default 0);

alter table focus_guests add (band_streamID integer default 0);

update focus_guests
set band_streamID = streamID
where band_streamID is null;

update focus_guests upd 
set band_streamID = (
    SELECT CASE min(c.couponID)
          WHEN 1 THEN 12
          WHEN 2 THEN 12
          ELSE 11
       END AS STAFF
FROM focus_guest_coupons gc
     inner join focus_coupons c on (gc.couponID = c.couponID) 
     where gc.guestID = upd.guestID)
where streamID = 11;

commit;

create or replace view view_wristbands as
SELECT g.title, g.firstName, g.lastName, g.guestID, bg.bookingID, g.gender, g.dateOfBirth, g.phone, 
g.email, g.churchID, g.age, g.firstFocus, g.schoolYear, g.band_streamID, ifnull(hbbp.band_print_date,0) as band_print_date,
left(case when c.churchID != 0 THEN c.churchDesc ELSE g.churchOther END, 40) as churchDesc,
case when band_streamID in (13,14) THEN
(select upper(group_concat(DISTINCT case a.accommodationDesc when 'Full week pass' then a.accommodationDesc 
else left(a.accommodationDesc, 3) end SEPARATOR ', ')) 
from focus_booking_accommodation ba 
left outer join focus_accommodation a on (ba.accommodationID = a.accommodationID)
where ba.bookingID = bg.bookingID) 
ELSE null END as days,
    (select max(phone) 
     from focus_guests inner_g
     inner join focus_booking_groups inner_bg on (inner_g.guestID = inner_bg.guestID)
     where inner_bg.bookingID = bg.bookingID and inner_g.lastName sounds like g.lastName
     group by bg.bookingID) as family_phone,
     (select max(phone) 
     from focus_guests inner_g
     inner join focus_booking_groups inner_bg on (inner_g.guestID = inner_bg.guestID)
     where inner_bg.bookingID = bg.bookingID
     group by bg.bookingID) as booking_phone,
     streamDesc, streamColour, statusDesc,
     case band_streamID when 13 then 1 when 14 then 1 else 0 end as offsite, b.statusID
FROM focus_guests g
left outer join htb_bands.band_prints hbbp on (g.guestID = hbbp.guestID)
inner join focus_booking_groups bg on (g.guestID = bg.guestID)
inner join focus_band_streams s on (g.band_streamID = s.streamID)
inner join focus_bookings b on (bg.bookingID = b.bookingID)
inner join focus_status st on (b.statusID = st.statusID)
left outer join focus_churches c on (g.churchID = c.churchID)
where active = 1
ORDER BY lastName Asc;

UPDATE `htb_focus`.`focus_churches` SET `churchDesc`='HTB Brompton Road' WHERE `churchID`='1';
UPDATE `htb_focus`.`focus_churches` SET `churchDesc`='HTB Onslow Square' WHERE `churchID`='2';
UPDATE `htb_focus`.`focus_churches` SET `churchDesc`='HTB Queens Gate' WHERE `churchID`='3';


create table focus_band_streams as select * from focus_streams;

insert into focus_band_streams (streamID, streamDesc, StreamColour)
values
(12, 'Staff', 'Red');

insert into focus_band_streams (streamID, streamDesc, StreamColour)
values
(13, 'Offsite Day Pass', 'Pink');

insert into focus_band_streams (streamID, streamDesc, StreamColour)
values
(14, 'Offsite Full Week', 'Orange');

update focus_guests
set band_streamID = streamID
where (band_streamID is null or band_streamID = 0);



update focus_guests set band_streamID = 13
where guestID in (
SELECT bg.guestID
FROM focus_booking_groups bg
inner join focus_bookings b on (bg.bookingID = b.bookingID)
inner join focus_status st on (b.statusID = st.statusID)
left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID)
where ba.accommodationID in (13,14,15,16,16,18));

update focus_guests set band_streamID = 14
where guestID in (
SELECT bg.guestID
FROM focus_booking_groups bg
inner join focus_bookings b on (bg.bookingID = b.bookingID)
inner join focus_status st on (b.statusID = st.statusID)
left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID)
where ba.accommodationID in (19));


update focus_guests upd 
set band_streamID = (
    SELECT CASE min(c.couponID)
          WHEN 1 THEN 12
          WHEN 2 THEN 12
          ELSE 11
       END AS STAFF
FROM focus_guest_coupons gc
     inner join focus_coupons c on (gc.couponID = c.couponID) 
     where gc.guestID = upd.guestID)
where streamID = 11;

update focus_guests set band_streamID = streamID
where band_streamID in (13,14)
and age <= 17;

commit;





