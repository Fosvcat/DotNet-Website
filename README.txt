This project is a website demo built with .NET + SQLite.

============================================================
GEEKSPACE
============================================================

Geekspace is a web-based learning platform focused on cybersecurity and
technology education. It brings together articles, videos, virtual
labs, simulations, and self-assessments in one place, and lets
registered members discuss and rate each piece of content, test their
knowledge with quick quizzes, and hold open conversations in a general
discussion area.

This README is a plain-language guide to what the site does and how to
use it.

------------------------------------------------------------
TABLE OF CONTENTS
------------------------------------------------------------

1.  Getting Started
2.  Test Accounts
3.  User Roles
4.  Browsing Resources
5.  Categories
6.  Questions
7.  Forum
8.  Search
9.  Random Browsing
10. Reading an Article
11. Comments, Replies & Discussions
12. Likes & Dislikes
13. Notifications
14. My Activity
15. Managing Content (Admin / Root)
16. User Management (Admin / Root)
17. Account Settings
18. Two-Factor Authentication (TOTP)
19. Look & Feel

------------------------------------------------------------
1. GETTING STARTED
------------------------------------------------------------

1) Open the project folder in a terminal.

2) Restore and build the project:

       dotnet build

3) Apply the database migrations (creates the local SQLite database):

       dotnet ef database update

4) Run the site:

       dotnet watch run

5) Open the address shown in the terminal (typically
   http://localhost:5159) in a browser.

The first time the site starts, it seeds a handful of sample
categories and resources automatically, so there is content to browse
right away. It's also worth registering (or logging into) the
root@fosvcat.com and admin@fosvcat.com accounts once on a fresh copy
of the site, to make sure their Root/Admin roles are assigned
correctly.

------------------------------------------------------------
2. TEST ACCOUNTS
------------------------------------------------------------

The following accounts are already registered and can be used to
explore each role's experience:

  Email                  Password    Role
  root@fosvcat.com       #Root123    Root
  admin@fosvcat.com      #Admin123   Admin
  User@fosvcat.com       #User123    User
  Mark@fosvcat.com       #Mark123    User

New visitors can also register their own account from the "Register"
link in the top-right corner. New accounts are given an automatically
generated username based on their email address, which can be changed
at any time -- see "Account Settings".

------------------------------------------------------------
3. USER ROLES
------------------------------------------------------------

The site has four tiers of access:

- Visitor (not logged in) -- can browse all published resources,
  categories, and the forum, but cannot post comments, like/dislike,
  take quizzes, or manage content.

- User -- a registered member. Can access all resources; can comment,
  reply, like/dislike, post in the forum, take quizzes, and manage
  their own account and comments; and can receive notifications.

- Admin -- has all User permissions, plus can create, edit, and delete
  resources and categories, and moderate comments and forum posts.
  Can delete other Users' posts, promote a User to Admin, and
  ban/unban or delete a User.

- Root -- there is only one Root account. Has the highest level of
  access. In addition to all Admin permissions, can demote an Admin
  back to User, moderate and delete posts from any account, and ban
  or delete any account except their own.

------------------------------------------------------------
4. BROWSING RESOURCES
------------------------------------------------------------

The Resources page lists every published learning resource on the
site -- articles, videos, virtual labs, simulations, and
self-assessments. Each entry shows its type, category, and creation
date. Anyone can view resources; only Admins and Root can create,
edit, or delete them.

------------------------------------------------------------
5. CATEGORIES
------------------------------------------------------------

The Categories page groups resources by subject area (for example,
"Cybersecurity Basics" or "CTF & Practical Hacking"). Each category
has a short description and links through to the resources that
belong to it.

------------------------------------------------------------
6. QUESTIONS
------------------------------------------------------------

The Questions section is a separate area of the site with short,
self-contained multiple-choice quizzes (for example, "Understanding
the CIA Triad" or "TCP/IP Fundamentals"). Signed-in members can answer
each quiz and immediately see which answers were correct. Only
logged-in users can open a quiz. Every quiz also has its own
discussion section at the bottom, using the same comment, reply, and
voting tools as everywhere else on the site.

------------------------------------------------------------
7. FORUM
------------------------------------------------------------

The Forum is a general discussion board, separate from any specific
resource. Any signed-in member can start a new discussion thread
there; it uses the exact same comment, reply, and voting tools
described below.

------------------------------------------------------------
8. SEARCH
------------------------------------------------------------

The search box in the top navigation bar searches resource titles,
descriptions, and content. Press Enter to see a results page in the
same format as the main Resources list.

------------------------------------------------------------
9. RANDOM BROWSING
------------------------------------------------------------

The "Random Browsing" button on the homepage jumps straight to a
randomly chosen published resource -- a quick way to discover
something new.

------------------------------------------------------------
10. READING AN ARTICLE
------------------------------------------------------------

Each resource's detail page opens with a header showing the title, a
short lead-in, and at-a-glance details: publish date, an estimated
reading time, its category, and its resource type. Below that is any
attached media (an embedded video or image, centered on the page), an
optional downloadable file (shown as a labeled button that downloads
it directly), and the full article body.

Article bodies support Markdown formatting -- headings, bold/italic
text, lists, links, images, code blocks, tables, and quotes are all
rendered properly when the page is viewed. If an article's body uses
heading formatting (#, ##, ###), a table of contents automatically
appears alongside the article, linking to each section; articles
without headings simply don't show one.

At the bottom of the article, alongside the Edit/Back to List buttons,
a row of share buttons lets you copy the article's link or share it
directly to Telegram, WhatsApp, LinkedIn, or X.

------------------------------------------------------------
11. COMMENTS, REPLIES & DISCUSSIONS
------------------------------------------------------------

Every resource page, quiz page, and the Forum have a discussion
section at the bottom:

- Signed-in members can post a comment using the text box provided.
- Click "Reply" under any comment to respond directly to it. The
  reply appears in the list with a small quoted preview of the
  original comment above it -- click the quote to jump back to what
  it's replying to.
- Comment authors can delete their own comments at any time; Admins
  and Root can delete other people's comments according to the role
  permissions described above.

------------------------------------------------------------
12. LIKES & DISLIKES
------------------------------------------------------------

Every comment has thumbs-up and thumbs-down buttons with a running
count, visible to everyone. Signed-in members can vote once per
comment -- clicking the opposite button changes your vote, and
clicking your own vote again removes it.

------------------------------------------------------------
13. NOTIFICATIONS
------------------------------------------------------------

Signed-in members receive a notification whenever someone replies to
their comment or likes/dislikes it. The bell icon in the top
navigation bar shows an unread count. Click it to open the
Notifications page, where each entry shows who acted, what they did,
and where (a resource, a quiz, or the Forum) -- with buttons to jump
to the relevant comment ("View"), mark a single notification as read
("Read"), or remove it ("Delete"). "Read All" and "Delete All"
buttons at the top handle the whole list at once.

------------------------------------------------------------
14. MY ACTIVITY
------------------------------------------------------------

Every signed-in member has a personal "Manage Activity" page (linked
from the account menu) listing every comment they've posted, with
quick links to jump to each one or delete it.

For Admins and Root, the "Manage Activity" page shows every user's
activity instead, for moderation purposes. They can delete other
people's comments according to the role permissions described above.

------------------------------------------------------------
15. MANAGING CONTENT (ADMIN / ROOT)
------------------------------------------------------------

Admins and Root can:

- Create, edit, and delete learning resources (title, type, category,
  description, Markdown article body, media, an optional downloadable
  file, and publish status).
- Create, edit, and delete categories.
- Delete any comment or forum post according to their role's
  permissions.

When logged in as Admin or Root, these controls automatically appear
in the navigation and on every list/detail page -- otherwise they
don't appear.

------------------------------------------------------------
16. USER MANAGEMENT (ADMIN / ROOT)
------------------------------------------------------------

The "Manage Users" page (in the account menu) lists every registered
account, including their username, email, role, and account status.

- Admin can promote a User to Admin, but cannot demote other Admins
  back to User. Admin can ban or delete User accounts, but cannot ban
  or delete other Admin accounts.
- Root can freely change any account between Admin and User, ban or
  unban any Admin or User account, and delete any Admin or User
  account. Root accounts themselves can never be banned, deleted, or
  demoted through this screen.
- A banned account can no longer log in -- attempting to sign in
  shows a message explaining that the account has been banned. If
  that account is already signed in elsewhere, it will be
  automatically signed out within 30 seconds of being banned.
- For security reasons, deleting an account clears its ban status
  first.
- Promotion, demotion, ban, unban, and deletion actions all require
  confirmation before taking effect.

------------------------------------------------------------
17. ACCOUNT SETTINGS
------------------------------------------------------------

From the account menu, "Manage Account" lets a member update:

- Username -- freely editable at any time (must be unique; the site
  will say so if the name is already taken). New accounts are
  automatically given a starter username based on their email
  address, which can be changed right away.
- Email, Password, and Phone number.

------------------------------------------------------------
18. TWO-FACTOR AUTHENTICATION (TOTP)
------------------------------------------------------------

Also under "Manage Account", members can turn on TOTP-based
two-factor authentication using any standard authenticator app
(Google Authenticator, Microsoft Authenticator, etc.) by scanning the
QR code shown on the setup page. Once enabled, logins will ask for a
one-time code from the app in addition to the password.

------------------------------------------------------------
19. LOOK & FEEL
------------------------------------------------------------

- Light / Dark Theme -- the circular icon button in the top
  navigation bar switches the whole site between light and dark mode,
  with a circular reveal animation. The choice is remembered on the
  device for future visits.
- Animated background -- the homepage, Resources, Categories,
  Questions, and Forum pages have a subtle animated background that
  adapts to the current theme (a technical grid in light mode, a
  drifting particle field in dark mode), with a small burst of motion
  wherever you click on empty page background.
- Homepage typewriter effect -- the homepage headline types out a
  rotating set of taglines.
