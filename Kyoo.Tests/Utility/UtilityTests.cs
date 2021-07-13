using System;
using System.Linq.Expressions;
using Kyoo.Models;
using Xunit;

namespace Kyoo.Tests
{
	public class UtilityTests
	{
		[Fact]
		public void IsPropertyExpression_Tests()
		{
			Expression<Func<Show, int>> member = x => x.ID;
			Expression<Func<Show, object>> memberCast = x => x.ID;

			Assert.False(Utility.IsPropertyExpression(null));
			Assert.True(Utility.IsPropertyExpression(member));
			Assert.True(Utility.IsPropertyExpression(memberCast));

			Expression<Func<Show, object>> call = x => x.GetID("test");
			Assert.False(Utility.IsPropertyExpression(call));
		}
		
		[Fact]
		public void GetPropertyName_Test()
		{
			Expression<Func<Show, int>> member = x => x.ID;
			Expression<Func<Show, object>> memberCast = x => x.ID;

			Assert.Equal("ID", Utility.GetPropertyName(member));
			Assert.Equal("ID", Utility.GetPropertyName(memberCast));
			Assert.Throws<ArgumentException>(() => Utility.GetPropertyName(null));
		}
	}
}