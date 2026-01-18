# CobaltSky
> Named after the default Windows Phone 8.1 accent color

> [!WARNING]
> This app is in an alpha stage, meaning there might be bugs, especially with compiling the Release build, which crashes upon launching, please report any bugs to this GitHub page!

A feature-rich, simple Bluesky client for Windows Phone 8.1.

# Feature list / Roadmap
This is the current list of features that are implemented, this may change in the future
- Setup flow
   - [x] Signing into a Bluesky account
   - [x] Setting up feed settings
- Client
   - [x] Loading posts
      - [x] Text in the post
      - [ ] Embeds in the post
         - [x] Images
         - [ ] Videos
            - This will be a hard task since Bluesky uses the .m3u8 format, which is not officially supported on Windows Phone, if anyone could get it working before me, please submit a pull request with your code included!
         - [ ] Link embeds
      - [x] Rich text (Links, mentions, hashtags)
         - This does not function when you click them just yet.
      - [ ] Pagination / Infinite scrolling
   - [x] Uploading posts (Text only)
      - Other forms of uploading posts are not complete.
   - [x] Search features
      - [x] Searching for users
      - [x] Showing trending topics
         - [ ] Showing posts for trending topics
   - [ ] Notifications
      - [ ] Push notifications
   - [ ] Direct messaging
   - [ ] Account viewing
   - [ ] Feed support
   - [ ] List support
   - [ ] Saved posts

# Credits
- OmegaAOL: Cerulean (Used this as a point of reference of the API)
