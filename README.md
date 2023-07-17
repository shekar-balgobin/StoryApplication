# Santander - Developer Coding Test

Using ASP.NET Core, implement a RESTful API to retrieve the details of the first n 'best stories' from the Hacker News API, where _n_ is specified by the caller to the API.

The Hacker News API is documented here: https://github.com/HackerNews/API.

The IDs for the "best stories" can be retrieved from this URI: https://hacker-news.firebaseio.com/v0/beststories.json.

The details for an individual story ID can be retrieved from this URI: https://hacker-news.firebaseio.com/v0/item/21233041.json (in this case for the story with ID 21233041).

The API should return an array of the first n 'best stories' as returned by the Hacker News API, sorted by their score in a descending order, in the form:

```json
[
    {
        "title": "A uBlock Origin update was rejected from the Chrome Web Store",
        "uri": "https://github.com/uBlockOrigin/uBlock-issues/issues/745",
        "postedBy": "ismaildonmez",
        "time": "2019-10-12T13:43:01+00:00",
        "score": 1716,
        "commentCount": 572
    },
    { ... },
...
]
```

In addition to the above, your API should be able to efficiently service large numbers of requests without risking overloading of the Hacker News API.

You should share a public repository with us, that should include a README.md file which describes how to run the application, any assumptions you have made, and any enhancements or changes you would make, given the time.

# Solution

The underlying problem is that the Hacker News API is very expensive when brokering a request to get the latest 'best stories'. The call is comprised of one request to get the list of IDs, followed typically by 200 requests to get each individual story. This takes around ~30 seconds, so doing this on a request by request basis will overload the Hackew News API and also provide a poor user experience.

The logical thing to do would be to cache the stories and serve requests from the cache directly. Cacheing introduces another problem in that the data will eventually become stale.

The solution described below is based on a couple of processes that run in the background of the web server to maintain the cache, whilst still allowing the server to respond to incoming requests.

![Overview](/Image/Overview.png)

## Periodic Refresh Background Task

This background task runs periodically to get a complete set of stories (this may take ~30 seconds). This task maintains a __buffered__ cache; buffered meaning that two caches exist internally comprised of a __read__ and __write__ dictionary. This task _writes_ the stories to the __write__ cache only, before switching caches (toggling). After a configuable period (default 5 minutes) the operation is repeated.

We have two caches to allow this process to refresh the __write__ cache whilst still allowing requests to be served from the __read__ cache. The GET method on the Story controller makes use of the __read__ cache when serving requests.

## Periodic Update Background Task

To complement the 'refresh background task' we also have an 'update background task' to address the problem of the cache becoming stale.

The Hacker News API contains a method to get the list of all updates. This can be used to poll for updates to the stories. This task simply keeps all 'story updates' in a collection that is only reset when signaled from the refresh task following a toggle. After a configuable period (default 1 minute) the operation is repeated.

## GET Story

The GET n stories method combines the result of __read__ cache with that of the list of updates and orders the stories based on their score property in descending order.

# Getting Started

 - Clone the repository.
 - Run from Visual Studio with F5.
 - A console window will appear showing informational logs. It is a good idea to dispaly this so you know when stories and updates are available.
 - A Swagger page will appear that will allow you to run the GET method. Simply set the number of stories. Please note that on start up the caches are empty, so the method will respond with a 503 until filled.

# Known Issues

- The caches are in memory for every instance of the web server. Therefore, it doesn't scale well when there are many instances of this service. We should instead use a distributed cache to solve this issue.
- Two caches are used. An alternative would be to use only one but then we will have to deal with concurrency and locking, so I chose this method to get around these sorts of problems and code complexity.
- I would have like to experiment more with the periodic settings to fine tune the service.
- I would have liked to expand the number of tests to catch edge cases.