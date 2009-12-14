﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MyBlog.Domain.Repositories;
using MyBlog.Domain;
using System.Transactions;

namespace MyBlog.WebSite
{
    public partial class ShowPost : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                using (var ws = Config.GetWorkspace())
                {
                    var postRepository = new PostRepository(ws);
                    int postId = this.GetCurrentPostId();
                    Post post = postRepository.FindById(postId);

                    this.BindPost(post);
                }
            }
        }

        private void BindPost(Post post)
        {
            this.litPublishDate.Text = Utils.FormatDate(post.PublishDate.Value);
            this.litSubject.Text = Utils.FormatText(post.Subject);
            this.litBody.Text = Utils.FormatText(post.Body);
            litCommentCount.Text = post.Comments.Count().ToString();

            //Up to the page to decide how to order the comments
            this.repReplies.DataSource = post.Comments.OrderBy(c => c.CreationDate);
            this.repReplies.DataBind();

            pnlReply.Visible = post.CommentsEnabled;
        }

        private int GetCurrentPostId()
        {
            return int.Parse(this.Request["postId"]);
        }

        //TODO: fix this
        public string FormatCategories(object o)
        {
            IEnumerable<PostCategoryLink> links = o as IEnumerable<PostCategoryLink>;

            var strings = links.Select(l => l.PostCategory.Name).ToArray();

            return string.Join(", ", strings);
        }

        protected string FormatCreationDate(object o)
        {
            DateTime dt = (DateTime)o;
            return Utils.FormatDateTime(dt);
        }

        protected void btnSubmitComment_Click(object sender, EventArgs e)
        {
            int postId = this.GetCurrentPostId();
            string userName = txtUserName.Text;
            string userEmail  = txtUserEmail.Text;
            string userWebSite = txtUserWebSite.Text;
            string comment = txtComment.Text;

            ReplyToPost(postId, userName, userEmail, userWebSite, comment);   

            txtComment.Text = "";
        }

        private void ReplyToPost(int postId, string userName, string userEmail, string userWebSite, string comment)
        {
            using (TransactionScope scope = new TransactionScope())
            using (var ws = Config.GetWorkspace())
            {
                var messageBus = Config.GetMessageBus(ws);
                var postRepository = new PostRepository(ws);

                Post post = postRepository.FindById(postId);

                post.ReplyTo(messageBus, userName, userEmail, userWebSite, comment);
                ws.Commit();

                BindPost(post);
                scope.Complete();
            }
        }
    }
}
