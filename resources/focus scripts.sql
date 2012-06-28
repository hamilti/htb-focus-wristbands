SELECT  * FROM focus_guests g
inner join focus_booking_groups bg on (g.guestID = bg.guestID)
left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID  )
left outer join focus_accommodation a on (ba.accommodationID = a.accommodationID)
where g.guestid <= 4183
order by bg.bookingID asc, guesttypeid desc;


select count(*), inner_1.guestid from (
SELECT g.guestid FROM focus_guests g
inner join focus_booking_groups bg on (g.guestID = bg.guestID)
left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID  )
left outer join focus_accommodation a on (ba.accommodationID = a.accommodationID)
where g.guestid <= 4183
order by bg.bookingID asc, guesttypeid desc
) inner_1 
group by guestid having count(*) > 1;


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

select count(*) from focus_guests;

select * from focus_payments;

select * from focus_guest_types;

select * from view_wristbands;


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

# set state to printing for all non staff
update focus_guests as target
set target.letter_stage = 1
where target.guestID in (
SELECT guestID from focus_booking_groups bg 
inner join focus_bookings b on (bg.bookingID = b.bookingID)
left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID  )
left outer join focus_accommodation a on (ba.accommodationID = a.accommodationID)
where ba.accommodationID not in (13,14,15,16,16,18,19)
and b.statusID not in (0, 4, 7, 8, 11, 6)) 
and target.staff = 0
and target.letter_stage = 0;

# set state to printing for all non staff
update focus_guests as target
set target.letter_stage = 1
where target.guestID in (
SELECT guestID from focus_booking_groups bg 
inner join focus_bookings b on (bg.bookingID = b.bookingID)
left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID  )
left outer join focus_accommodation a on (ba.accommodationID = a.accommodationID)
where ba.accommodationID not in (13,14,15,16,16,18,19)
and b.statusID not in (0, 4, 7, 8, 11, 6)) 
and target.staff = 1
and target.letter_stage = 0;

commit;

#after printing reset
update focus_guests
set letter_stage = 2
where letter_stage = 1;

# Setup wristband printing

alter table focus_guests add (band_print_date integer default 0);

create or replace view view_wristbands as
SELECT bg.bookingID, g.guestID, g.guestTypeID, g.title, g.firstName, g.lastName, g.gender, g.dateOfBirth, 
g.addressOne, g.addressTwo, g.addressThree, g.town, g.county, g.country, g.postcode, g.phone, 
g.email, g.churchID, g.churchOther, g.pastorateID, g.age, g.firstFocus, g.student, g.schoolYear, 
g.relationshipID, g.specialNeeds, g.streamID, g.bursary, g.coachTicket, g.carPass, g.drupalID, 
g.active, g.bookedDate, g.donationAmount, g.bursaryAmount, g.staff, g.letter_stage, g.band_print_date,
c.churchDesc
FROM focus_guests g
inner join focus_booking_groups bg on (g.guestID = bg.guestID)
inner join focus_bookings b on (bg.bookingID = b.bookingID)
left outer join focus_booking_accommodation ba on (bg.bookingID = ba.bookingID)
left outer join focus_accommodation a on (ba.accommodationID = a.accommodationID)
left outer join focus_churches c on (g.churchID = c.churchID)
where ba.accommodationID not in (13,14,15,16,16,18,19)
and b.statusID not in (0, 4, 7, 8, 11, 6)
and g.band_print_date = 0
ORDER BY bg.bookingID ASC, g.guestID ASC;


