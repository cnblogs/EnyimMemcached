﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching.Memcached.Results.StatusCodes;
using Xunit;

namespace Enyim.Caching.Tests
{
    public class MemcachedClientGetTests : MemcachedClientTestsBase
    {
        [Fact]
        public void When_Getting_Existing_Item_Value_Is_Not_Null_And_Result_Is_Successful()
        {
            var key = GetUniqueKey("get");
            var value = GetRandomString();
            var storeResult = Store(key: key, value: value);
            StoreAssertPass(storeResult);

            var getResult = _client.ExecuteGet(key);
            GetAssertPass(getResult, value);
        }

        [Fact]
        public void When_Getting_Item_For_Invalid_Key_HasValue_Is_False_And_Result_Is_Not_Successful()
        {
            var key = GetUniqueKey("get");

            var getResult = _client.ExecuteGet(key);
            Assert.True(getResult.StatusCode == (int)StatusCodeEnums.NotFound, "Invalid status code");
            GetAssertFail(getResult);
        }

        [Fact]
        public void When_TryGetting_Existing_Item_Value_Is_Not_Null_And_Result_Is_Successful()
        {
            var key = GetUniqueKey("get");
            var value = GetRandomString();
            var storeResult = Store(key: key, value: value);
            StoreAssertPass(storeResult);

            object temp;
            var getResult = _client.ExecuteTryGet(key, out temp);
            GetAssertPass(getResult, temp);
        }

        [Fact]
        public void When_Generic_TryGetting_Existing_Item_Value_Is_Not_Null_And_Result_Is_Successful()
        {
            var key = GetUniqueKey("get");
            var value = GetRandomString();
            var storeResult = Store(key: key, value: value);
            StoreAssertPass(storeResult);

            string temp;
            var getResult = _client.ExecuteTryGet(key, out temp);
            GetAssertPass(getResult, temp);
        }

        [Fact]
        public void When_Generic_Getting_Existing_Item_Value_Is_Not_Null_And_Result_Is_Successful()
        {
            var key = GetUniqueKey("get");
            var value = GetRandomString();
            var storeResult = Store(key: key, value: value);
            StoreAssertPass(storeResult);

            var getResult = _client.ExecuteGet<string>(key);
            Assert.True(getResult.Success, "Success was false");
            Assert.True(getResult.Cas > 0, "Cas value was 0");
            Assert.True((getResult.StatusCode ?? 0) == 0, "StatusCode was neither 0 nor null");
            Assert.Equal(value, getResult.Value);
        }

        [Fact]
        public void When_Getting_Multiple_Keys_Result_Is_Successful()
        {
            var keys = GetUniqueKeys().Distinct();
            foreach (var key in keys)
            {
                Store(key: key, value: "Value for" + key);
            }

            var dict = _client.ExecuteGet(keys);
            Assert.Equal(keys.Count(), dict.Keys.Count);

            foreach (var key in dict.Keys)
            {
                Assert.True(dict[key].Success, "Get failed for key: " + key);
            }
        }

        [Fact]
        public void When_Getting_Byte_Result_Is_Successful()
        {
            var key = GetUniqueKey("Get");
            const byte expectedValue = 1;
            Store(key: key, value: expectedValue);
            var getResult = _client.ExecuteGet(key);
            GetAssertPass(getResult, expectedValue);
        }

        [Fact]
        public void When_Getting_SByte_Result_Is_Successful()
        {
            var key = GetUniqueKey("Get");
            const sbyte expectedValue = 1;
            Store(key: key, value: expectedValue);
            var getResult = _client.ExecuteGet(key);
            GetAssertPass(getResult, expectedValue);
        }

        [Fact]
        public async Task When_Getting_Empty_String_Async_Result_Is_Successful()
        {
            var key = GetUniqueKey("Get");
            var empty = string.Empty;
            Store(key: key, value: empty);
            var getResult = await _client.GetValueAsync<string>(key);
            Assert.Equal(getResult, empty);
        }

        [Fact]
        public async Task GetValueOrCreateAsyncTest()
        {
            var key = "GetValueOrCreateAsyncTest_" + Guid.NewGuid();
            var posts1 = await _client.GetValueOrCreateAsync(
                key,
                10,
                async () => await GenerateValue());
            Assert.NotNull(posts1);

            var posts2 = await _client.GetValueAsync<IEnumerable<BlogPost>>(key);
            Assert.NotNull(posts2);

            Assert.Equal(posts1.First().Title, posts2.First().Title);
        }

        private Task<IEnumerable<BlogPost>> GenerateValue()
        {
            var posts = new List<BlogPost>()
            {
                new BlogPost{ Title = "test title 1", Body = "test body 1" },
                new BlogPost{ Title = "test title 2", Body = "test body 2" },
            };

            return Task.FromResult(posts.AsEnumerable());
        }
    }

    internal class BlogPost
    {
        public string Title { get; set; }
        public string Tags { get; set; }
        public string Body { get; set; }
    }
}

#region [ License information          ]
/* ************************************************************
 * 
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2012 Couchbase, Inc.
 *    @copyright 2012 Attila Kiskó, enyim.com
 *    
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *    
 *        http://www.apache.org/licenses/LICENSE-2.0
 *    
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *    
 * ************************************************************/
#endregion