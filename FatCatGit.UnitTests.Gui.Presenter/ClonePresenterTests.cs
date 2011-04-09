﻿using System.Configuration;
using FatCatGit.Gui.Presenter.Exceptions;
using FatCatGit.Gui.Presenter.Presenters;
using FatCatGit.Gui.Presenter.Views;
using NUnit.Framework;
using Rhino.Mocks;

namespace FatCatGit.UnitTests.Gui.Presenter
{
    [TestFixture]
    [Category("Presentation")]
    public class ClonePresenterTests : BasePresenterTests
    {
        private static string DestinationLocation
        {
            get { return ConfigurationManager.AppSettings["DestinationLocation"]; }
        }

        private const string TestRepository = @"C:\SomeTestRepository";
        private const string TestRepositoryWithSubFolder = @"C:\SomeTestRepository\SubFolderTest";

        private CloneView SetUpClonePresenterTestWithRepositry(string repositoryToClone, string expectedDestinationFolder)
        {
            var cloneView = Mocks.DynamicMock<CloneView>();

            cloneView.Expect(v => v.RepositoryToClone).Return(repositoryToClone);

            cloneView.DestinationFolder = expectedDestinationFolder;
            LastCall.PropertyBehavior();

            Mocks.ReplayAll();

            return cloneView;
        }

        private void VerifyGitRepositoryName(string gitUrl)
        {
            string expectedDestinationFolder = string.Format("{0}\\{1}", DestinationLocation, "FatCatGit");

            CloneView cloneView = SetUpClonePresenterTestWithRepositry(gitUrl, expectedDestinationFolder);

            var presenter = new ClonePresenter(cloneView);

            presenter.SetDestinationFolder(DestinationLocation);

            Assert.That(cloneView.DestinationFolder, Is.EqualTo(expectedDestinationFolder));
        }

        [Test]
        public void IfDestinationFolderBlankItIsNotSet()
        {
            var cloneView = Mocks.DynamicMock<CloneView>();

            cloneView.Expect(v => v.RepositoryToClone).IgnoreArguments().Repeat.Never();

            cloneView.DestinationFolder = null;
            LastCall.PropertyBehavior();
            LastCall.IgnoreArguments();
            LastCall.Repeat.Never();

            Mocks.ReplayAll();

            var presenter = new ClonePresenter(cloneView);

            presenter.SetDestinationFolder(null);

            Assert.That(cloneView.DestinationFolder, Is.Null);
        }

        [Test]
        public void DestinationFolderWillNotAddExtraSlashAtEndOfTheFolder()
        {
            string expectedDestinationFolder = string.Format("{0}\\{1}", DestinationLocation, "SubFolderTest");

            CloneView cloneView = SetUpClonePresenterTestWithRepositry(TestRepositoryWithSubFolder, expectedDestinationFolder);

            var presenter = new ClonePresenter(cloneView);

            presenter.SetDestinationFolder(DestinationLocation);

            Assert.That(cloneView.DestinationFolder, Is.EqualTo(expectedDestinationFolder));
        }

        [Test]
        public void SpecificyDestionationFolderWillUseRepositoryNameAsSubFolder()
        {
            string expectedDestinationFolder = string.Format("{0}\\{1}", DestinationLocation, "SomeTestRepository");

            CloneView cloneView = SetUpClonePresenterTestWithRepositry(TestRepository, expectedDestinationFolder);

            var presenter = new ClonePresenter(cloneView);

            presenter.SetDestinationFolder(DestinationLocation);

            Assert.That(cloneView.DestinationFolder, Is.EqualTo(expectedDestinationFolder));
        }

        [Test]
        public void IfDirectoryEndsInRepoNameDoNotAdd()
        {
            string expectedDestinationFolder = string.Format("{0}\\{1}\\", DestinationLocation, "SomeTestRepository");

            CloneView cloneView = SetUpClonePresenterTestWithRepositry(TestRepository, expectedDestinationFolder);

            var presenter = new ClonePresenter(cloneView);

            presenter.SetDestinationFolder(expectedDestinationFolder);

            Assert.That(cloneView.DestinationFolder, Is.EqualTo(expectedDestinationFolder));
        }

        [Test]
        public void SpecificyDestionationFolderWillUseRepositoryNameAsSubFolderWhenRepoUnderParentFolder()
        {
            string expectedDestinationFolder = string.Format("{0}\\{1}", DestinationLocation, "SubFolderTest");

            CloneView cloneView = SetUpClonePresenterTestWithRepositry(TestRepositoryWithSubFolder, expectedDestinationFolder);

            var presenter = new ClonePresenter(cloneView);

            presenter.SetDestinationFolder(DestinationLocation);

            Assert.That(cloneView.DestinationFolder, Is.EqualTo(expectedDestinationFolder));
        }

        [Test]
        public void WillDetermineRepoFromGitHubUrl()
        {
            const string gitUrl = @"git@github.com:DavidBasarab/FatCatGit.git";

            VerifyGitRepositoryName(gitUrl);
        }

        [Test]
        public void WillDetermineRepoNameFromRemoteRepoistory()
        {
            const string gitUrl = @"git@127.0.0.1:FatCatGit.git";

            VerifyGitRepositoryName(gitUrl);
        }

        [Test]
        public void WillDetermineSshRepoName()
        {
            const string gitUrl = @"ssh://[user@]host.xz[:port]/path/to/FatCatGit.git/";

            VerifyGitRepositoryName(gitUrl);
        }

        [Test]
        public void WillDetermineHttpsRepoName()
        {
            const string gitUrl = @"http[s]://host.xz[:port]/path/to/FatCatGit.git/";

            VerifyGitRepositoryName(gitUrl);
        }

        [Test]
        public void WillDetermineFtpsRepoName()
        {
            const string gitUrl = @"ftp[s]://host.xz[:port]/path/to/FatCatGit.git/";

            VerifyGitRepositoryName(gitUrl);
        }
    }
}