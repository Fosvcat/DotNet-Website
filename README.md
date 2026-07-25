# Geekspace

Geekspace is a web-based learning platform for cybersecurity and technology education. It brings together articles, videos, virtual labs, simulations, and self-assessments in one place, and lets registered members discuss and rate each piece of content, ask questions, and hold open conversations on a general discussion board.

This README is a plain-language guide to what the site does and how to use it. Technical implementation details are covered separately in the project's assignment documentation.

---

## Table of Contents

- [Getting Started](#getting-started)
- [Test Accounts](#test-accounts)
- [User Roles](#user-roles)
- [Browsing Resources](#browsing-resources)
- [Categories](#categories)
- [Search](#search)
- [Random Browsing](#random-browsing)
- [Reading an Article](#reading-an-article)
- [The Forum](#the-forum)
- [Comments, Replies & Discussions](#comments-replies--discussions)
- [Likes & Dislikes](#likes--dislikes)
- [Notifications](#notifications)
- [My Activity](#my-activity)
- [Managing Content (Admin / Root)](#managing-content-admin--root)
- [User Management (Admin / Root)](#user-management-admin--root)
- [Account Settings](#account-settings)
- [Two-Factor Authentication (TOTP)](#two-factor-authentication-totp)
- [Light / Dark Theme](#light--dark-theme)

---

## Getting Started

1. Open the project folder in a terminal.
2. Restore and build the project:
   ```bash
   dotnet build
   ```
3. Apply the database migrations (creates the local SQLite database):
   ```bash
   dotnet ef database update
   ```
4. Run the site:
   ```bash
   dotnet watch run
   ```
5. Open the address shown in the terminal (typically `http://localhost:5159`) in a browser.

The first time the site starts, it seeds a handful of sample categories and resources automatically, so there is content to browse right away.

---

## Test Accounts

The following accounts are already registered and can be used to explore every role's experience:

| Email | Password | Role |
|---|---|---|
| root@fosvcat.com | #Root123 | Root |
| admin@fosvcat.com | #Admin123 | Admin |
| User@fosvcat.com | #User123 | User |
| Mark@fosvcat.com | #Mark123 | User |

New visitors can also register their own account from the **Register** link in the top-right corner.

---

## User Roles

Geekspace has three tiers of access:

- **Visitor (not logged in)** — can browse all published resources, categories, and the forum, but cannot comment, vote, or manage content.
- **User** — a registered member. Can comment, reply, like/dislike, post in the forum, and manage their own account and comments.
- **Admin** — everything a User can do, plus creating, editing, and deleting resources and categories, moderating comments and forum posts, and promoting Users to Admin.
- **Root** — the highest level of access. Everything an Admin can do, plus demoting Admins, deleting any account (except other Root accounts), and moderating content from anyone, including Admins.

---

## Browsing Resources

The **Resources** page lists every published learning resource on the site — articles, videos, virtual labs, simulations, and self-assessments. Each entry shows its type, category, and creation date. Anyone can view resources; only Admins and Root can create, edit, or delete them.

## Categories

The **Categories** page groups resources by subject area (for example, "Cybersecurity Basics" or "CTF & Practical Hacking"). Each category has a short description and links through to the resources that belong to it.

## Search

The search box in the top navigation bar searches resource titles, descriptions, and content. Press **Enter** to see a results page in the same format as the main Resources list.

## Random Browsing

The **Random Browsing** button on the homepage jumps straight to a randomly chosen published resource — a quick way to discover something new.

## Reading an Article

Each resource's detail page shows its title, type, category, and publish date, followed by any attached media (an embedded video or image), an optional downloadable file, a short description, and the full article body. Article bodies support **Markdown formatting** — headings, bold/italic text, lists, links, images, code blocks, tables, and quotes are all rendered properly when the page is viewed.

## The Forum

The **Forum** is a general discussion board, separate from any specific resource. Any signed-in member can start a new discussion thread there; it uses the exact same comment, reply, and voting tools described below.

## Comments, Replies & Discussions

Every resource page and the Forum have a discussion section at the bottom:

- Signed-in members can post a comment using the text box provided.
- Click **Reply** under any comment to respond directly to it. The reply appears in the list with a small quoted preview of the original comment above it — click the quote to jump back to what it's replying to.
- Comment authors (and Admins/Root) can **Delete** their own or others' comments, depending on role permissions described above.

## Likes & Dislikes

Every comment has thumbs-up and thumbs-down buttons with a running count, visible to everyone. Signed-in members can vote once per comment — clicking the opposite button switches your vote, and clicking your own vote again removes it. Voting happens instantly without reloading the page.

## Notifications

Signed-in members receive a notification whenever someone replies to their comment or likes/dislikes it. The bell icon in the top navigation bar shows an unread count. Click it to open the **Notifications** page, where each entry shows who acted, what they did, and where — with buttons to jump to the relevant comment (**View**), mark a single notification as read (**Read**), or remove it (**Delete**). **Read All** and **Delete All** buttons at the top handle the whole list at once.

## My Activity

Every signed-in member has a personal **Manage Activity** page (linked from the account menu) listing every comment they've posted, with quick links to jump to each one or delete it. Admins and Root see everyone's activity here instead, for moderation purposes.

## Managing Content (Admin / Root)

Admins and Root can:

- Create, edit, and delete learning resources (title, type, category, description, article body, media, downloadable file, and publish status).
- Create, edit, and delete categories.
- Delete any comment or forum post according to their role's permissions.

These controls appear automatically in the navigation and on each list/detail page when logged in with sufficient permissions — no separate admin area to hunt for.

## User Management (Admin / Root)

The **Manage Users** page (from the account menu) lists every registered account, their username, email, and role.

- **Admin** can promote a User to Admin.
- **Root** can also demote an Admin back to a regular User, and can delete any Admin or User account (Root accounts themselves can never be deleted or demoted through this screen).
- Promotion and demotion actions ask for confirmation before taking effect.

## Account Settings

From the account menu, **Manage Account** lets a member update their:

- **Username** — freely editable at any time (must be unique; the site will say so if the name is already taken). New accounts are automatically given a starter username based on their email address, which can be changed right away.
- **Email**, **Password**, and **Phone number**.

## Two-Factor Authentication (TOTP)

Also under **Manage Account**, members can turn on TOTP-based two-factor authentication using any standard authenticator app (Google Authenticator, Microsoft Authenticator, etc.) by scanning the QR code shown on the setup page. Once enabled, logins will ask for a one-time code from the app in addition to the password.

## Light / Dark Theme

The circular icon button in the top navigation bar switches the whole site between light and dark mode. The choice is remembered on the device for future visits.
