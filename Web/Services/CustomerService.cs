using System;
using System.Threading.Tasks;
using Web.Models;
using SharedLibrary;
using Microsoft.EntityFrameworkCore;
using static Web.Models.DbTables;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Web.Services
{
    public class CustomerService
    {
        private readonly ApplicationContext _context;

        public CustomerService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<int> UpdateCustomer(CustomerReport customer)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (customer == null)
                    {
                        return -1; //error customer is null
                    }

                    var customerDetails = await _context.Customers.FindAsync(customer.CustId);
                    if (customerDetails == null)
                    {
                        return -2; //error customer is not in the database
                    }

                    //Update customer details
                    customerDetails.CompanyName = customer.CompanyName;
                    customerDetails.ContactName = customer.ContactName;
                    customerDetails.ContactTitle = customer.ContactTitle;
                    customerDetails.Address = customer.Address;
                    customerDetails.City = customer.City;
                    customerDetails.Region = customer.Region;
                    customerDetails.PostalCode = customer.PostalCode;
                    customerDetails.Country = customer.Country;
                    customerDetails.Phone = customer.Phone;
                    customerDetails.Fax = customer.Fax;

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();

                    return 1; //Success
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an exception
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<int> CreateCustomer(CustomerReport customer)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (customer == null)
                    {
                        return -1; //error customer is null
                    }

                    if (customer.CustId != 0)
                    {
                        //Update existing customer
                        await UpdateCustomer(customer);
                        return customer.CustId;
                    }

                    // Add new customer to db
                    var newCustomer = new Customer
                    {
                        CompanyName = customer.CompanyName,
                        ContactName = customer.ContactName,
                        ContactTitle = customer.ContactTitle,
                        Address = customer.Address,
                        City = customer.City,
                        Region = customer.Region,
                        PostalCode = customer.PostalCode,
                        Country = customer.Country,
                        Phone = customer.Phone,
                        Fax = customer.Fax
                    };
                    _context.Customers.Add(newCustomer);

                    // Reload the newCustomer entity to get the updated CustId
                    _context.Entry(newCustomer).Reload();

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();

                    return newCustomer.CustId; //Success
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an exception
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<List<CustomerReport>> GetListOfCustomers()
        {
            // Retrieve customer list
            var customers = _context.Customers;
            var customerList = customers.Select(c => new CustomerReport
            {
                CustId = c.CustId,
                CompanyName = c.CompanyName,
                ContactName = c.ContactName,
                ContactTitle = c.ContactTitle,
                Address = c.Address,
                City = c.City,
                Region = c.Region,
                PostalCode = c.PostalCode,
                Country = c.Country,
                Phone = c.Phone,
                Fax = c.Fax
            }).ToList();

            return customerList;
        }
    }
}
