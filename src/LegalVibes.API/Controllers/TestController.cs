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

            // 2. Create a project for this user
            var project = new Project
            {
                Name = "Trademark Registration",
                Description = "Client trademark registration project",
                Status = ProjectStatus.InProgress,
                Type = ProjectType.TrademarkApplication,
                DueDate = DateTime.Now.AddMonths(1),
                ClientName = "Acme Inc",
                ClientReference = "ACM-2025-001",
                UserId = user.Id, // This sets up the relationship
                TrademarkName = "AcmeCorp",
                TrademarkDescription = "Technology solutions provider",
                GoodsAndServices = "Software and consulting services",
                SpecialConsiderations = "Urgent processing needed",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.SaveChangesAsync();

            // 3. Create a document for this project
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
                ProjectId = project.Id, // This sets up the relationship
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Documents.AddAsync(document);
            await _unitOfWork.SaveChangesAsync();

            // 4. Retrieve everything to demonstrate relationships
            var retrievedUser = await _unitOfWork.Users.GetByIdAsync(user.Id);
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
                    retrievedUser?.CreatedAt // Note: This comes from BaseEntity
                },
                Projects = userProjects.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Status,
                    p.CreatedAt, // From BaseEntity
                    p.UserId // Navigation property
                }),
                Documents = projectDocuments.Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.Status,
                    d.CreatedAt, // From BaseEntity
                    d.ProjectId // Navigation property
                })
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
} 