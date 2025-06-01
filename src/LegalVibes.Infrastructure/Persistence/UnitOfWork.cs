using LegalVibes.Application.Interfaces;
using LegalVibes.Domain.Entities;
using LegalVibes.Infrastructure.Persistence.Repositories;

namespace LegalVibes.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IRepository<User> _userRepository;
    private IRepository<Project> _projectRepository;
    private IRepository<Document> _documentRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<User> Users => 
        _userRepository ??= new GenericRepository<User>(_context);

    public IRepository<Project> Projects =>
        _projectRepository ??= new GenericRepository<Project>(_context);

    public IRepository<Document> Documents =>
        _documentRepository ??= new GenericRepository<Document>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
} 