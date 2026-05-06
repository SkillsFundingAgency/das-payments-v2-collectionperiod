Feature: Open Collection Years

As a consumer of the Collection Period API 
I would like to get information on the current status of collection years and collection periods


Scenario: Open Collection Year
	Given that the collection period has opened recently
	When a request is made to get the open collection years from the Collection Periods API
	Then the response should contain the current collection year

Scenario: Multiple Open Collection Years
	Given that the collection period R01 is currently open
	And collection period R13 is also open
	When a request is made to get the open collection years from the Collection Periods API
	Then the response should contain the both open collection years

Scenario: In-between Collection Periods
	Given that the collection period has recently completed
	But the next collection period has not yet been opened
	When a request is made to get the open collection years from the Collection Periods API
	Then the response should contain the current collection year
