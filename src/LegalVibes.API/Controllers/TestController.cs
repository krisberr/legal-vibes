using LegalVibes.Application.Interfaces;
using LegalVibes.Domain.Entities;
using LegalVibes.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LegalVibes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public TestController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost("test-relationships")]
    public async Task<IActionResult> TestRelationships()
    {
        try
        {
            // 1. Create a user
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                IsActive = true,
                CompanyName = "Legal Corp",
                JobTitle = "IP Lawyer",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // 2. Create a client for this user
            var client = new Client
            {
                Name = "Acme Inc",
                Email = "contact@acme.com",
                PhoneNumber = "555-123-4567",
                CompanyName = "Acme Corporation",
                Address = "123 Business St, City, State 12345",
                UserId = user.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Clients.AddAsync(client);
            await _unitOfWork.SaveChangesAsync();

            // 3. Create a project for this user and client
            var project = new Project
            {
                Name = "Trademark Registration",
                Description = "Client trademark registration project",
                Status = ProjectStatus.InProgress,
                Type = ProjectType.TrademarkApplication,
                DueDate = DateTime.Now.AddMonths(1),
                ReferenceNumber = "ACM-2025-001",
                UserId = user.Id,
                ClientId = client.Id, // New: Link to Client instead of string fields
                TrademarkName = "AcmeCorp",
                TrademarkDescription = "Technology solutions provider",
                GoodsAndServices = "Software and consulting services",
                SpecialConsiderations = "Urgent processing needed",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.SaveChangesAsync();

            // 4. Create a document for this project
            var document = new Document
            {
                Name = "Trademark Application",
                Description = "Initial trademark application document",
                Type = DocumentType.TrademarkApplication,
                Status = DocumentStatus.Draft,
                ContentType = "application/pdf",
                StoragePath = "/storage/documents/trademark-app.pdf",
                SizeInBytes = 1024 * 1024, // 1MB
                Version = "1.0",
                IsAIGenerated = true,
                GenerationPrompt = "Generate trademark application for AcmeCorp",
                AIModel = "GPT-4",
                ProjectId = project.Id,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Documents.AddAsync(document);
            await _unitOfWork.SaveChangesAsync();

            // 5. Retrieve everything to demonstrate relationships
            var retrievedUser = await _unitOfWork.Users.GetByIdAsync(user.Id);
            var userClients = await _unitOfWork.Clients.FindAsync(c => c.UserId == user.Id);
            var userProjects = await _unitOfWork.Projects.FindAsync(p => p.UserId == user.Id);
            var projectDocuments = await _unitOfWork.Documents.FindAsync(d => d.ProjectId == project.Id);

            // Return the results
            return Ok(new
            {
                User = new
                {
                    retrievedUser?.Id,
                    retrievedUser?.FirstName,
                    retrievedUser?.LastName,
                    retrievedUser?.Email,
                    retrievedUser?.CreatedAt
                },
                Clients = userClients.Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Email,
                    c.CompanyName,
                    c.CreatedAt,
                    c.UserId
                }),
                Projects = userProjects.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Status,
                    p.ReferenceNumber, // Updated field name
                    p.CreatedAt,
                    p.UserId,
                    p.ClientId // New: Reference to Client
                }),
                Documents = projectDocuments.Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.Status,
                    d.CreatedAt,
                    d.ProjectId
                })
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
} 