create table if not exists  sales_managers (
    id serial primary key not null,
    name varchar(250) not null,
    languages varchar(100)[],
    products varchar(100)[],
    customer_ratings varchar(100)[]
);

create table if not exists slots (
    id serial primary key not null,
    start_date timestamptz not null,
    end_date timestamptz not null,
    booked boolean not null default false,
    sales_manager_id int not null references sales_managers(Id)
);

CREATE INDEX idx_slots_sales_manager_id ON slots (sales_manager_id);
CREATE INDEX idx_slots_start_date_booked ON slots (start_date, booked);
CREATE INDEX idx_slots_end_date_booked ON slots (end_date, booked);

CREATE INDEX idx_sales_managers_arrays ON sales_managers USING GIN (languages, customer_ratings, products);

insert into sales_managers (name, languages, products, customer_ratings) values ('Seller 1', '{"German"}', '{"SolarPanels"}', '{"Bronze"}');
insert into sales_managers (name, languages, products, customer_ratings) values ('Seller 2', '{"German", "English"}', '{"SolarPanels", "Heatpumps"}', '{"Gold","Silver","Bronze"}');
insert into sales_managers (name, languages, products, customer_ratings) values ('Seller 3', '{"German", "English"}', '{"Heatpumps"}', '{"Gold","Silver","Bronze"}');

insert into slots (sales_manager_id, booked, start_date, end_date) values (1, false, '2024-05-03T10:30Z', '2024-05-03T11:30Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (1, true,  '2024-05-03T11:00Z', '2024-05-03T12:00Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (1, false, '2024-05-03T11:30Z', '2024-05-03T12:30Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (2, false, '2024-05-03T10:30Z', '2024-05-03T11:30Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (2, false, '2024-05-03T11:00Z', '2024-05-03T12:00Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (2, false, '2024-05-03T11:30Z', '2024-05-03T12:30Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (3, true,  '2024-05-03T10:30Z', '2024-05-03T11:30Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (3, false, '2024-05-03T11:00Z', '2024-05-03T12:00Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (3, false, '2024-05-03T11:30Z', '2024-05-03T12:30Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (1, false, '2024-05-04T10:30Z', '2024-05-04T11:30Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (1, false, '2024-05-04T11:00Z', '2024-05-04T12:00Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (1, true,  '2024-05-04T11:30Z', '2024-05-04T12:30Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (2, true,  '2024-05-04T10:30Z', '2024-05-04T11:30Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (2, false, '2024-05-04T11:00Z', '2024-05-04T12:00Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (2, true,  '2024-05-04T11:30Z', '2024-05-04T12:30Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (3, true,  '2024-05-04T10:30Z', '2024-05-04T11:30Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (3, false, '2024-05-04T11:00Z', '2024-05-04T12:00Z');
insert into slots (sales_manager_id, booked, start_date, end_date) values (3, false, '2024-05-04T11:30Z', '2024-05-04T12:30Z');


CREATE VIEW vw_available_slots AS
WITH sorted_slots AS (
    SELECT
        s.id,
        s.start_date,
        s.end_date,
        s.booked,
        s.sales_manager_id,
        sm.languages,
        sm.customer_ratings,
        sm.products,
        LAG(s.booked) OVER (PARTITION BY s.sales_manager_id ORDER BY s.start_date) AS prev_booked,
        LAG(s.end_date) OVER (PARTITION BY s.sales_manager_id ORDER BY s.start_date) AS prev_end_date,
        LEAD(s.booked) OVER (PARTITION BY s.sales_manager_id ORDER BY s.start_date) AS next_booked,
        LEAD(s.start_date) OVER (PARTITION BY s.sales_manager_id ORDER BY s.start_date) AS next_start_date
    FROM slots s
    JOIN sales_managers sm ON s.sales_manager_id = sm.id
)
SELECT id, start_date, end_date, sales_manager_id, languages, products, customer_ratings
FROM sorted_slots
WHERE (
    booked = false AND
    (prev_booked IS NULL OR prev_booked = false OR prev_end_date <= start_date) AND
    (next_booked IS NULL OR next_booked = false OR next_start_date >= end_date)
);
