# Geekspace

Geekspace is a web-based learning platform for cybersecurity and technology education. It brings together articles, videos, virtual labs, simulations, and self-assessments in one place, and lets registered members discuss and rate each piece of content, test their knowledge with quick quizzes, and hold open conversations on a general discussion board.

This README is a plain-language guide to what the site does and how to use it. Technical implementation details are covered separately in the project's assignment documentation.

---

## Table of Contents

- [Getting Started](#getting-started)
- [Test Accounts](#test-accounts)
- [User Roles](#user-roles)
- [Browsing Resources](#browsing-resources)
- [Categories](#categories)
- [Questions](#questions)
- [The Forum](#the-forum)
- [Search](#search)
- [Random Browsing](#random-browsing)
- [Reading an Article](#reading-an-article)
- [Comments, Replies & Discussions](#comments-replies--discussions)
- [Likes & Dislikes](#likes--dislikes)
- [Notifications](#notifications)
- [My Activity](#my-activity)
- [Managing Content (Admin / Root)](#managing-content-admin--root)
- [User Management (Admin / Root)](#user-management-admin--root)
- [Account Settings](#account-settings)
- [Two-Factor Authentication (TOTP)](#two-factor-authentication-totp)
- [Look & Feel](#look--feel)

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

The first time the site starts, it seeds a handful of sample categories and resources automatically, so there is content to browse right away. It's also worth registering (or logging into) the `root@fosvcat.com` and `admin@fosvcat.com` accounts once on a fresh copy of the site so their Root/Admin roles are assigned correctly.

---

## Test Accounts

The following accounts are already registered and can be used to explore every role's experience:

| Email | Password | Role |
|---|---|---|
| root@fosvcat.com | #Root123 | Root |
| admin@fosvcat.com | #Admin123 | Admin |
| User@fosvcat.com | #User123 | User |
| Mark@fosvcat.com | #Mark123 | User |

New visitors can also register their own account from the **Register** link in the top-right corner. New accounts are given an automatically generated username (based on the email address) that can be changed at any time — see [Account Settings](#account-settings).

---

## User Roles

Geekspace has three tiers of access:

- **Visitor (not logged in)** — can browse all published resources, categories, and the forum, but cannot comment, vote, take quizzes, or manage content.
- **User** — a registered member. Can comment, reply, like/dislike, post in the forum, take quizzes, and manage their own account and comments.
- **Admin** — everything a User can do, plus creating, editing, and deleting resources and categories, moderating comments and forum posts, and promoting Users to Admin.
- **Root** — the highest level of access. Everything an Admin can do, plus demoting Admins, banning or deleting any account (except other Root accounts), and moderating content from anyone, including Admins.

---

## Browsing Resources

The **Resources** page lists every published learning resource on the site — articles, videos, virtual labs, simulations, and self-assessments. Each entry shows its type, category, and creation date. Anyone can view resources; only Admins and Root can create, edit, or delete them.

## Categories

The **Categories** page groups resources by subject area (for example, "Cybersecurity Basics" or "CTF & Practical Hacking"). Each category has a short description and links through to the resources that belong to it.

## Questions

The **Questions** section is a separate area of the site with short, self-contained multiple-choice quizzes (for example, "Understanding the CIA Triad" or "TCP/IP Fundamentals"). Signed-in members can answer each quiz and immediately see which answers were correct. Only logged-in users can open a quiz — visiting a quiz page while signed out redirects to the login page. Every quiz also has its own discussion section at the bottom, using the same comment, reply, and voting tools as everywhere else on the site.

## The Forum

The **Forum** is a general discussion board, separate from any specific resource. Any signed-in member can start a new discussion thread there; it uses the exact same comment, reply, and voting tools described below.

## Search

The search box in the top navigation bar searches resource titles, descriptions, and content. Press **Enter** to see a results page in the same format as the main Resources list.

## Random Browsing

The **Random Browsing** button on the homepage jumps straight to a randomly chosen published resource — a quick way to discover something new.

## Reading an Article

Each resource's detail page opens with a header showing the title, a short lead-in, and at-a-glance details: publish date, an estimated reading time, its category, and its resource type. Below that is any attached media (an embedded video or image, centered on the page), an optional **downloadable file** (shown as a labeled button that downloads it directly), and the full article body.

Article bodies support **Markdown formatting** — headings, bold/italic text, lists, links, images, code blocks, tables, and quotes are all rendered properly when the page is viewed. If an article's body uses heading formatting (`#`, `##`, `###`), a **table of contents** automatically appears alongside the article, linking to each section; articles without headings simply don't show one.

At the bottom of the article, alongside the Edit/Back to List buttons, a row of **share buttons** lets you copy the article's link or share it directly to Telegram, WhatsApp, LinkedIn, or X.

## Comments, Replies & Discussions

Every resource page, quiz page, and the Forum have a discussion section at the bottom:

- Signed-in members can post a comment using the text box provided.
- Click **Reply** under any comment to respond directly to it. The reply appears in the list with a small quoted preview of the original comment above it — click the quote to jump back to what it's replying to.
- Comment authors (and Admins/Root) can **Delete** their own or others' comments, depending on role permissions described above.

## Likes & Dislikes

Every comment has thumbs-up and thumbs-down buttons with a running count, visible to everyone. Signed-in members can vote once per comment — clicking the opposite button switches your vote, and clicking your own vote again removes it. Voting happens instantly without reloading the page.

## Notifications

Signed-in members receive a notification whenever someone replies to their comment or likes/dislikes it. The bell icon in the top navigation bar shows an unread count. Click it to open the **Notifications** page, where each entry shows who acted, what they did, and where (a resource, a quiz, or the Forum) — with buttons to jump to the relevant comment (**View**), mark a single notification as read (**Read**), or remove it (**Delete**). **Read All** and **Delete All** buttons at the top handle the whole list at once.

## My Activity

Every signed-in member has a personal **Manage Activity** page (linked from the account menu) listing every comment they've posted, with quick links to jump to each one or delete it. Admins and Root see everyone's activity here instead, for moderation purposes.

## Managing Content (Admin / Root)

Admins and Root can:

- Create, edit, and delete learning resources (title, type, category, description, Markdown article body, media, an optional downloadable file, and publish status).
- Create, edit, and delete categories.
- Delete any comment or forum post according to their role's permissions.

These controls appear automatically in the navigation and on each list/detail page when logged in with sufficient permissions — no separate admin area to hunt for.

## User Management (Admin / Root)

The **Manage Users** page (from the account menu) lists every registered account, their username, email, role, and account status.

- **Admin** can promote a User to Admin, and can ban or delete a User account.
- **Root** can also demote an Admin back to a regular User, ban or unban any Admin or User account, and delete any Admin or User account. Root accounts themselves can never be banned, deleted, or demoted through this screen.
- A **banned** account can no longer log in — attempting to sign in shows a message explaining the account has been banned. If the account is already signed in elsewhere, it's automatically signed out shortly after being banned.
- Deleting an account clears its ban status first, as a safeguard.
- Promotion, demotion, ban, unban, and deletion actions all ask for confirmation before taking effect.

## Account Settings

From the account menu, **Manage Account** lets a member update their:

- **Username** — freely editable at any time (must be unique; the site will say so if the name is already taken). New accounts are automatically given a starter username based on their email address, which can be changed right away.
- **Email**, **Password**, and **Phone number**.

## Two-Factor Authentication (TOTP)

Also under **Manage Account**, members can turn on TOTP-based two-factor authentication using any standard authenticator app (Google Authenticator, Microsoft Authenticator, etc.) by scanning the QR code shown on the setup page. Once enabled, logins will ask for a one-time code from the app in addition to the password.

## Look & Feel

- **Light / Dark Theme** — the circular icon button in the top navigation bar switches the whole site between light and dark mode, with a circular reveal animation. The choice is remembered on the device for future visits.
- **Animated background** — the homepage, Resources, Categories, Questions, and Forum pages have a subtle animated background that adapts to the current theme (a technical grid in light mode, a drifting particle field in dark mode), with a small burst of motion wherever you click on empty page background.
- **Homepage typewriter effect** — the homepage headline types out a rotating set of taglines.
