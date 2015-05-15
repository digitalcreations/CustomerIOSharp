# Customer.io API C# implementation

Implementation of [Customer.io](http://www.customer.io)'s write-only API for .NET.

## Installation

Using Nuget:

    Install-Package CustomerIOSharp

## Usage

We recommend you implement the `ICustomerFactory` interface yourself. Here is a suggested implementation used in one of my projects:

```cs
    public class CustomerFactory : ICustomerFactory
    {
        public ICustomerDetails GetCustomerDetails()
        {
            var user = MembershipHelper.GetCurrentUser();
            return user == null 
                ? null 
                : user.AsCustomer();
        }

        public string GetCustomerId()
        {
            var user = MembershipHelper.GetCurrentUser();
            return user == null
                ? null
                : user.UserId.ToString();
        }
    }
```

Then instantiate `CustomerIo` and call `IdentifyAsync()`:

```cs
var customerIo = new CustomerIo(
    "siteid", 
    "apikey", 
    new CustomerFactory());
await customerIo.IdentifyAsync();
```

You should most likely only call `IdentifyAsync()` whenever the user logs in or is changed.

If you do not want to implement `ICustomerFactory`, you can simply provide the required details to each call:

```cs
await customerIo.IdentifyAsync(User.AsCustomer());
```

Whatever you do, you will have to provide customer data implementing `ICustomerDetails`. It has only two required properties, but any properties you supply in your implementation will be forwarded to customer.io:

```cs
class Customer : ICustomerDetails 
{
    // these two fields are required:
    string Id { get; set; }
    string Email { get; set; }
    // these are my custom fields:
    string FirstName { get; set; }
    string LastName { get; set;
}
```

### Custom events

Track a custom event by calling `TrackEvent()`. It takes an event name as the first parameter, the second parameter is any serializable object. Note that it automatically posts using the correct customer (using `ICustomerFactory`), but you can also supply your own.

```cs
await customerIo.TrackEventAsync("signup", new {
	Group = "trial",
	Referrer = "email campaign"
});

// this event has no data and uses a different customer
await customerIo.TrackEventAsync("signup", customerId: "foo");
```

Note that these two variables will (by default) be camelcased before being sent to customer.io, so `group` and `referrer` will be available in your transactional campaigns.

## License

Dual licensed under the MIT and the GPL license.

You don’t have to do anything special to choose one license or the other and you don’t have to notify anyone which license you are using.

### MIT license

The MIT License (MIT)

Copyright (c) 2013 [Digital Creations AS](http://www.digitalcreations.no).

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

### GPL license

Copyright (c) 2013 [Digital Creations AS](http://www.digitalcreations.no).

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.
