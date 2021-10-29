﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Moq;
using Taarafo.Core.Models.Posts;
using Taarafo.Core.Models.Posts.Exceptions;
using Xunit;

namespace Taarafo.Core.Tests.Unit.Services.Foundations.Posts
{
    public partial class PostServiceTests
    {
        [Fact]
        public async Task ShouldThrowNotFoundExceptionIfPostIsNotFoundAndLogItAsync()
        {
            // given
            Guid randomPostId = Guid.NewGuid();
            Guid inputPostId = randomPostId;
            Post noPost = null;

            var notFoundPostException =
                new NotFoundPostException(inputPostId);

            var expectedPostValidationException =
                new PostValidationException(
                    notFoundPostException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noPost);

            // when
            ValueTask<Post> removePostByIdTask =
                this.postService.RemovePostByIdAsync(inputPostId);

            // then
            await Assert.ThrowsAsync<PostValidationException>(() =>
               removePostByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameValidationExceptionAs(
                    expectedPostValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeletePostAsync(It.IsAny<Post>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}