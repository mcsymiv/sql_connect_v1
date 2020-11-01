Feature: ManagePersons
	In order to manage my shop
	As a user
	I want to update my customers list
	Background:
	Given Connection with mcs DB is creted
@check_list_null
Scenario: Get Persons List
	When Send select command
	Then Get a list of all Persons
@add_person
Scenario: Add new Person
	Given Person data is generated
	When Send person data to Person table
	Then Person name is visible in table

@remove_person
Scenario: Remove person from list
	When Send delete command indicating Potter Harry
	Then Person is removed from DB