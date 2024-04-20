using AutoMapper;
using inventar_api.ArticleLocations.DTOs;
using inventar_api.ArticleLocations.Models;
using inventar_api.ArticleLocations.Repository.Interfaces;
using inventar_api.Articles.Models;
using inventar_api.Data;
using inventar_api.Locations.Models;
using Microsoft.EntityFrameworkCore;

namespace inventar_api.ArticleLocations.Repository;

public class ArticleLocationsRepository : IArticleLocationsRepository
{
    private AppDbContext _context;
    private IMapper _mapper;

    public ArticleLocationsRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ArticleLocation>> GetAllAsync()
    {
        return await _context.ArticleLocations.ToListAsync();
    }

    public async Task<ArticleLocation?> GetAsync(GetArticleLocationRequest request)
    {
        return await _context.ArticleLocations
            .Include(al => al.Article)
            .Include(al => al.Location)
            .FirstOrDefaultAsync(al =>
                al.Article.Code == request.ArticleCode &&
                al.Location.Code.Equals(request.LocationCode));
    }

    public async Task<ArticleLocation> CreateAsync(CreateArticleLocationRequest request)
    {
        Article article = (await _context.Articles.FirstOrDefaultAsync(a => a.Code == request.ArticleCode))!;
        Location location = (await _context.Locations.FirstOrDefaultAsync(a => a.Code.Equals(request.LocationCode)))!;

        ArticleLocation articleLocation = new ArticleLocation
        {
            ArticleId = article.Id,
            LocationId = location.Id,
            Count = request.Count
        };
        
        _context.ArticleLocations.Add(articleLocation);
        await _context.SaveChangesAsync();
        return articleLocation;
    }

    public async Task<ArticleLocation> UpdateAsync(UpdateArticleLocationRequest request)
    {
        Article article = (await _context.Articles.FirstOrDefaultAsync(a => a.Code == request.ArticleCode))!;
        Location location = (await _context.Locations.FirstOrDefaultAsync(a => a.Code.Equals(request.LocationCode)))!;
        
        ArticleLocation articleLocation = (await _context.ArticleLocations.FirstOrDefaultAsync(al =>
            al.ArticleId == article.Id && al.LocationId == location.Id))!;

        articleLocation.Count = request.Count;
        _context.ArticleLocations.Update(articleLocation);
        await _context.SaveChangesAsync();
        return articleLocation;
    }

    public async Task<ArticleLocation> DeleteAsync(DeleteArticleLocationRequest request)
    {
        ArticleLocation articleLocation = (await GetAsync(_mapper.Map<GetArticleLocationRequest>(request)))!;
        _context.ArticleLocations.Remove(articleLocation);
        await _context.SaveChangesAsync();
        return articleLocation;
    }
}