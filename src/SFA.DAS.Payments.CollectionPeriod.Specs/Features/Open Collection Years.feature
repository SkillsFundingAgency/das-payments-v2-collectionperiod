Feature: Open Collection Years

As a consumer of the Collection Period API 
I would like to get information on the current status of collection years and collection periods

Scenario: In-between Collection Periods
	Given that the collection period has recently completed
	But the next collection period has not yet been opened
	When a request is made to get the open collection years from the Collection Periods API
	Then the response should contain the current collection year
