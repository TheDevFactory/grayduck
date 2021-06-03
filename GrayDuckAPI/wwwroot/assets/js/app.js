var App = function () {

    function LoadMainMenus() {

        /*
            Plain JavaScript, no jQuery Needed
            Build Side bar Menu - Web and Mobile
        */
        document.getElementById('compactSidebar').innerHTML = ' \n' +
        '<div class="theme-logo"> <a href="dashboard.html"><img src="assets/img/duck-multi-size.ico" class="navbar-logo" alt="logo"></a> </div> \n' +
        '<ul class="menu-categories"> \n' +
        '   <li id="mainmenudashboard" class="menu"> \n' +
        '       <a href="#dashboard" data-active="false" class="menu-toggle"> \n' +
        '           <div class="base-menu"> \n' +
        '                <div class="base-icons"> \n' +
        '                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-home"><path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"></path><polyline points="9 22 9 12 15 12 15 22"></polyline></svg> \n' +
        '                </div> \n' +
        '            </div> \n' +
        '        </a> \n' +
        '        <div class="tooltip"><span>Home</span></div> \n' +
        '    </li> \n' +
        '    <li id="mainmenucontacts" class="menu"> \n' +
        '        <a href="#contact" data-active="false" class="menu-toggle"> \n' +
        '            <div class="base-menu"> \n' +
        '                <div class="base-icons"> \n' +
        '                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-users"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path><circle cx="9" cy="7" r="4"></circle><path d="M23 21v-2a4 4 0 0 0-3-3.87"></path><path d="M16 3.13a4 4 0 0 1 0 7.75"></path></svg> \n' +
        '                </div> \n' +
        '            </div> \n' +
        '        </a> \n' +
        '        <div class="tooltip"><span>Contacts</span></div> \n' +
        '    </li> \n' +
        '    <li id="mainmenuaccounts" class="menu"> \n' +
        '        <a href="#account" data-active="false" class="menu-toggle"> \n' +
        '            <div class="base-menu"> \n' +
        '                <div class="base-icons"> \n' +
        '                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-layers"><polygon points="12 2 2 7 12 12 22 7 12 2"></polygon><polyline points="2 17 12 22 22 17"></polyline><polyline points="2 12 12 17 22 12"></polyline></svg> \n' +
        '                </div> \n' +
        '            </div> \n' +
        '        </a> \n' +
        '        <div class="tooltip"><span>Accounts</span></div> \n' +
        '    </li> \n' +
        '    <li id="mainmenuadmin" class="menu"> \n' +
        '        <a href="#admin" data-active="false" class="menu-toggle"> \n' +
        '            <div class="base-menu"> \n' +
        '                <div class="base-icons"> \n' +
        '                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-settings"><circle cx="12" cy="12" r="3"></circle><path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1 0 2.83 2 2 0 0 1-2.83 0l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-2 2 2 2 0 0 1-2-2v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83 0 2 2 0 0 1 0-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1-2-2 2 2 0 0 1 2-2h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 0-2.83 2 2 0 0 1 2.83 0l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 2-2 2 2 0 0 1 2 2v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 0 2 2 0 0 1 0 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 2 2 2 2 0 0 1-2 2h-.09a1.65 1.65 0 0 0-1.51 1z"></path></svg>   \n' +
        '                </div> \n' +
        '            </div> \n' +
        '        </a> \n' +
        '        <div class="tooltip"><span>Administration</span></div> \n' +
        '   </li> \n' +
        '   <li id="mainmenustarterkit" class="menu"> \n' +
        '        <a href="#starter" data-active="false" class="menu-toggle"> \n' +
        '            <div class="base-menu"> \n' +
        '                <div class="base-icons"> \n' +
        '                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-terminal"><polyline points="4 17 10 11 4 5"></polyline><line x1="12" y1="19" x2="20" y2="19"></line></svg> \n' +
        '                </div> \n' +
        '            </div> \n' +
        '        </a> \n' +
        '        <div class="tooltip"><span>Starter Kit</span></div> \n' +
        '   </li> \n' +
        '</ul> \n' +
        '<div class="external-links"> \n' +
        '    <a href="documentation/index.html" target="_blank"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-book-open"><path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"></path><path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"></path></svg> \n' +
        '       <div class="tooltip"><span>Documentation</span></div> \n' +
        '    </a> \n' +
        '</div>';


        document.getElementById('compact_submenuSidebar').innerHTML = ' \n' +
        '  <div class="theme-brand-name"><a href="dashboard.html"> GrayDuck </a></div> \n' +
        '  <div class="submenu" id="dashboard"> \n' +
        '     <div class="category-info"> \n' +
        '        <h5>Home</h5> \n' +
        '        <p>Start managing you contacts and converting sales faster.</p> \n' +
        '    </div> \n' +
        '    <ul class="submenu-list" data-parent-element="#dashboard"> \n' +
        '        <li> \n' +
        '            <a href="dashboard.html"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-grid"><rect x="3" y="3" width="7" height="7"></rect><rect x="14" y="3" width="7" height="7"></rect><rect x="14" y="14" width="7" height="7"></rect><rect x="3" y="14" width="7" height="7"></rect></svg> Dashboard </a> \n' +
        '        </li> \n' +
        '     </ul> \n' +
        '  </div> \n' +
        '  <div class="submenu" id="contact"> \n' +
        '    <div class="category-info"> \n' +
        '       <h5>Contacts</h5> \n' +
        '       <p>Contact management with real results.</p> \n' +
        '    </div> \n' +
        '    <ul class="submenu-list" data-parent-element="#contact"> \n' +
        '       <li> \n' +
        '           <a href="contacts.html"><span class="icon"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-circle"><circle cx="12" cy="12" r="10"></circle></svg></span> Contact(s) </a> \n' +
        '       </li> \n' +
        '    </ul> \n' +
        '  </div> \n' +
        '  <div class="submenu" id="account"> \n' +
        '   <div class="category-info"> \n' +
        '      <h5>Accounts</h5> \n' +
        '      <p>Account management made easy.</p> \n' +
        '   </div> \n' +
        '   <ul class="submenu-list" data-parent-element="#account"> \n' +
        '       <li> \n' +
        '           <a href="accounts.html"><span class="icon"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-circle"><circle cx="12" cy="12" r="10"></circle></svg></span> Account(s) </a> \n' +
        '       </li> \n' +
        '   </ul> \n' +
        '  </div> \n' +
        '  <div class="submenu" id="admin"> \n' +
        '    <div class="category-info"> \n' +
        '        <h5>Administration</h5> \n' +
        '        <p>Manage your instance by adding your team memebers, settting permissions and customizing it to fit your needs.</p> \n' +
        '    </div> \n' +
        '    <ul class="submenu-list" data-parent-element="#admin"> \n' +
        '        <li> \n' +
        '           <a href="systemusers.html"><span class="icon"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-circle"><circle cx="12" cy="12" r="10"></circle></svg></span> Users </a> \n' +
        '        </li> \n' +
        '        <li> \n' +
        '           <a href="systemsecuritygroups.html"><span class="icon"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-circle"><circle cx="12" cy="12" r="10"></circle></svg></span> Security </a> \n' +
        '        </li> \n' +
        '        <li> \n' +
        '           <a href="systemcustomfields.html"><span class="icon"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-circle"><circle cx="12" cy="12" r="10"></circle></svg></span> Custom Fields </a> \n' +
        '        </li> \n' +
        '        <li> \n' +
        '           <a href="systemactivities.html"><span class="icon"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-circle"><circle cx="12" cy="12" r="10"></circle></svg></span> Activities </a> \n' +
        '        </li> \n' +
        '        <li> \n' +
        '           <a href="systemscoring.html"><span class="icon"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-circle"><circle cx="12" cy="12" r="10"></circle></svg></span> Scoring </a> \n' +
        '        </li> \n' +
        '        <li> \n' +
        '           <a href="systemlog.html"><span class="icon"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-circle"><circle cx="12" cy="12" r="10"></circle></svg></span> System Logs </a> \n' +
        '        </li> \n' +
        '    </ul> \n' +
        '  </div> \n' +
        '  <div class="submenu" id="starter"> \n' +
        '    <div class="category-info"> \n' +
        '        <h5>REST API</h5> \n' +
        '        <p>With starter kit, you can begin working with our REST API quickly.</p> \n' +
        '    </div> \n' +
        '    <ul class="submenu-list" data-parent-element="#starter"> \n' +
        '        <li> \n' +
        '           <a href="api/index.html" target="_blank"><span class="icon"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-circle"><circle cx="12" cy="12" r="10"></circle></svg></span> Swagger Documentation </a> \n' +
        '        </li> \n' +
        '        <li> \n' +
        '           <a href="platform_basics.html"><span class="icon"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-circle"><circle cx="12" cy="12" r="10"></circle></svg></span> Platform Basics </a> \n' +
        '       </li> \n' +
        '    </ul> \n' +
        '  </div>';


        /*
            Plain JavaScript, no jQuery Needed
            Build Copyright
        */
        var varDate = new Date();
        var varYear = varDate.getFullYear();
        document.getElementById('divFooterSection1').innerHTML = '<p class="">Copyright @ ' + varYear + ' <a target="_blank" href="https://grayduck.app">GrayDuck Software</a>, All rights reserved.</p>';
        document.getElementById('divFooterSection2').innerHTML = '<p class="">Coded with <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-heart"><path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"></path></svg></p>';

        /*
            Based on the page name we set teh active main menu
        */
        var sPath = window.location.pathname;
        //var sPage = sPath.substring(sPath.lastIndexOf('\\') + 1);
        var sPage = sPath.substring(sPath.lastIndexOf('/') + 1);
        //alert(sPage);


        switch (sPage) {
            case 'dashboard.html':
                $("#mainmenudashboard").addClass("active");
                break;
            case 'user_profile.html':
                $("#mainmenudashboard").addClass("active");
                break;

            case 'contacts.html':
                $("#mainmenucontacts").addClass("active");
                // code block
                break;

            case 'accounts.html':
                $("#mainmenuaccounts").addClass("active");
                // code block
                break;

            case 'systemlog.html':
                $("#mainmenuadmin").addClass("active");
                // code block
                break;
            case 'systemusers.html':
                $("#mainmenuadmin").addClass("active");
                // code block
                break;
            case 'systemsecuritygroups.html':
                $("#mainmenuadmin").addClass("active");
                // code block
                break;
            case 'systemcustomfields.html':
                $("#mainmenuadmin").addClass("active");
                // code block
                break;
            case 'systemactivities.html':
                $("#mainmenuadmin").addClass("active");
                // code block
                break;
            case 'systemscoring.html':
                $("#mainmenuadmin").addClass("active");
                // code block
                break;

            case 'platform_basics.html':
                $("#mainmenustarterkit").addClass("active");
                // code block
                break;
            default:
                $("#mainmenudashboard").addClass("active");
                // code block
        }

    }

    var MediaSize = {
        xl: 1200,
        lg: 992,
        md: 991,
        sm: 576
    };

    var ToggleClasses = {
        headerhamburger: '.toggle-sidebar',
        inputFocused: 'input-focused',
    };

    var Selector = {
        getBody: 'body',
        mainHeader: '.header.navbar',
        headerhamburger: '.toggle-sidebar',
        fixed: '.fixed-top',
        mainContainer: '.main-container',
        sidebar: '#sidebar',
        sidebarContent: '#sidebar-content',
        sidebarStickyContent: '.sticky-sidebar-content',
        ariaExpandedTrue: '#sidebar [aria-expanded="true"]',
        ariaExpandedFalse: '#sidebar [aria-expanded="false"]',
        contentWrapper: '#content',
        contentWrapperContent: '.container',
        mainContentArea: '.main-content',
        searchFull: '.toggle-search',
        overlay: {
            sidebar: '.overlay',
            cs: '.cs-overlay',
            search: '.search-overlay'
        }
    };

    var toggleFunction = {
        sidebar: function($recentSubmenu) {
            $('.sidebarCollapse').on('click', function (sidebar) {
                sidebar.preventDefault();

                get_CompactSubmenuShow = document.querySelector('#compact_submenuSidebar');
                get_overlay = document.querySelector('.overlay');
                get_mainContainer = document.querySelector('.main-container')
                if (get_CompactSubmenuShow.classList.contains('show') || get_CompactSubmenuShow.classList.contains('hide-sub') ) {
                    console.log('main1');

                    if (get_CompactSubmenuShow.classList.contains('show')) {
                        get_CompactSubmenuShow.classList.remove("show");
                        get_overlay.classList.remove("show");
                        get_CompactSubmenuShow.classList.add("hide-sub");
                        return;
                            console.log('1')
                    }
                    if (get_CompactSubmenuShow.classList.contains('hide-sub')) {

                        if (get_mainContainer.classList.contains('sidebar-closed')) {
                            get_mainContainer.classList.remove("sidebar-closed");
                            get_mainContainer.classList.add("sbar-open");
                            console.log('2')
                            return;
                        }
                        if (get_mainContainer.classList.contains('sbar-open')) {
                            get_mainContainer.classList.remove("sbar-open");
                            get_CompactSubmenuShow.classList.remove("hide-sub");
                            get_CompactSubmenuShow.classList.add("show");
                            get_overlay.classList.add("show");
                            console.log('3')
                            return;
                        }
                        $(Selector.mainContainer).addClass("sidebar-closed");
                    }

                } else  {
                    console.log('main2');
                    $(Selector.mainContainer).toggleClass("sidebar-closed");
                    $(Selector.mainContainer).toggleClass("sbar-open");
                    if (window.innerWidth <= 991) {
                        if (get_overlay.classList.contains('show')) {
                            get_overlay.classList.remove('show');
                        } else {
                            get_overlay.classList.add('show');
                        }
                    }
                    $('html,body').toggleClass('sidebar-noneoverflow');
                    $('footer .footer-section-1').toggleClass('f-close');
                }
            });
        },
        overlay: function() {
            $('.overlay').on('click', function () {
                // hide sidebar
                var windowWidth = window.innerWidth;
                if (windowWidth <= MediaSize.md) {
                    $('.main-container').addClass('sidebar-closed');
                }
                // hide overlay
                $('.overlay').removeClass('show');
                $('html,body').removeClass('sidebar-noneoverflow');

                $('#compact_submenuSidebar').removeClass('show');
                $('.menu.show').removeClass('show');
                $('body').removeClass('mini_bar-open');
            });
        },
        search: function() {
            $(Selector.searchFull).click(function(event) {
               $(this).parents('.search-animated').find('.search-full').addClass(ToggleClasses.inputFocused);
               $(this).parents('.search-animated').addClass('show-search');
               $(Selector.overlay.search).addClass('show');
               $(Selector.overlay.search).addClass('show');
            });

            $(Selector.overlay.search).click(function(event) {
               $(this).removeClass('show');
               $(Selector.searchFull).parents('.search-animated').find('.search-full').removeClass(ToggleClasses.inputFocused);
               $(Selector.searchFull).parents('.search-animated').removeClass('show-search');
            });
        },
        navbarShadow: function() {
            var getNav = document.querySelector('.navbar');
            var testDiv = document.querySelector(".main-content");
            document.addEventListener('scroll', function() {
                var doc = document.documentElement;
                var left = (window.pageXOffset || doc.scrollLeft) - (doc.clientLeft || 0);
                var top = (window.pageYOffset || doc.scrollTop)  - (doc.clientTop || 0);
                if (top >= testDiv.offsetTop) {
                    getNav.style.boxShadow = "0px 20px 20px rgba(126,142,177,0.12)";
                } else { getNav.removeAttribute("style"); }
            })
        }
    }

    var inBuiltfunctionality = {
        mainCatActivateScroll: function() {
            const ps = new PerfectScrollbar('.menu-categories', {
                wheelSpeed:.5,
                swipeEasing:!0,
                minScrollbarLength:40,
                maxScrollbarLength:100,
                suppressScrollX: true

            });
        },
        subCatScroll: function() {
            const submenuSidebar = new PerfectScrollbar('#compact_submenuSidebar', {
                wheelSpeed:.5,
                swipeEasing:!0,
                minScrollbarLength:40,
                maxScrollbarLength:100,
                suppressScrollX: true

            });
        },
        onSidebarClick: function() {
            var getMenu = document.querySelectorAll('.menu');

            for (var i = 0; i < getMenu.length; i++) {
                getMenu[i].addEventListener('click', function() {

                    get_body = document.querySelector('body');
                    getHref = this.querySelectorAll('.menu-toggle')[0].getAttribute('href');
                    getElement = document.querySelectorAll('#compact_submenuSidebar > ' + getHref)[0];
                    getMenuShowElement = document.querySelector('.menu.show');
                    getCompactSubmenu = document.querySelector('#compact_submenuSidebar');
                    getOverlayElement = document.querySelector('.overlay');
                    getElementActiveClass = document.querySelector('#compact_submenuSidebar > .show');
                    get_mainContainer = document.querySelector('.main-container')

                    if (getMenuShowElement) {
                        getMenuShowElement.classList.remove('show');
                        this.classList.add('show');
                    } else {
                        this.classList.add('show');
                    }

                    if (getCompactSubmenu) {
                        getCompactSubmenu.classList.add("show");
                        getOverlayElement.classList.add('show');
                        get_body.classList.add('mini_bar-open');
                        getCompactSubmenu.classList.remove('hide-sub');
                        get_mainContainer.classList.remove('sbar-open');
                    }

                    if (getElementActiveClass) {
                        getElementActiveClass.classList.remove("show");
                    }

                    getElement.className += " show";
                })
                getMenu[i].addEventListener('click', function(ev) {
                    ev.preventDefault();
                })
            }

        },
        preventScrollBody: function() {
            $('#compactSidebar, #compact_submenuSidebar').bind('mousewheel DOMMouseScroll', function(e) {
                var scrollTo = null;

                if (e.type == 'mousewheel') {
                    scrollTo = (e.originalEvent.wheelDelta * -1);
                }
                else if (e.type == 'DOMMouseScroll') {
                    scrollTo = 40 * e.originalEvent.detail;
                }

                if (scrollTo) {
                    e.preventDefault();
                    $(this).scrollTop(scrollTo + $(this).scrollTop());
                }
            });
        },
        languageDropdown: function() {
            var getDropdownElement = document.querySelectorAll('.more-dropdown .dropdown-item');
            for (var i = 0; i < getDropdownElement.length; i++) {
                getDropdownElement[i].addEventListener('click', function() {
                    document.querySelectorAll('.more-dropdown .dropdown-toggle > img')[0].setAttribute('src', 'assets/img/' + this.getAttribute('data-img-value') + '.png' );
                })
            }
        },
    }

    var _mobileResolution = {
        onRefresh: function() {
            var windowWidth = window.innerWidth;
            if ( windowWidth <= MediaSize.md ) {
                toggleFunction.sidebar();
            }
            if ( windowWidth < MediaSize.xl ) {
                inBuiltfunctionality.mainCatActivateScroll();
            }
        },
        
        onResize: function() {
            $(window).on('resize', function(event) {
                event.preventDefault();
                var windowWidth = window.innerWidth;
                if ( windowWidth <= MediaSize.md ) {
                }
                if ( windowWidth < MediaSize.xl ) {
                    inBuiltfunctionality.mainCatActivateScroll();
                }
            });
        }
        
    }

    var _desktopResolution = {
        onRefresh: function() {
            var windowWidth = window.innerWidth;
            if ( windowWidth > MediaSize.md ) {
                toggleFunction.sidebar(true);
            }
        },
        
        onResize: function() {
            $(window).on('resize', function(event) {
                event.preventDefault();
                var windowWidth = window.innerWidth;
                if ( windowWidth > MediaSize.md ) {
                }
            });
        }
        
    }

    function sidebarFunctionality() {
        function sidebarCloser() {

            if (window.innerWidth <= 1199 ) {


                if (!$('body').hasClass('alt-menu')) {

                    $('.main-container').removeClass('sbar-open');
                    $("#container").addClass("sidebar-closed");
                    $('.overlay').removeClass('show');
                    $('#compact_submenuSidebar').removeClass('show')

                } else {
                    $(".navbar").removeClass("expand-header");
                    $('.overlay').removeClass('show');
                    $('#container').removeClass('sbar-open');
                    $('html, body').removeClass('sidebar-noneoverflow');
                }

            } else if (window.innerWidth > 1199 ) {

                if (!$('body').hasClass('alt-menu')) {
                    $("#container").removeClass("sidebar-closed");
                    $('#container').removeClass('sbar-open');
                } else {
                    $('html, body').addClass('sidebar-noneoverflow');
                    $("#container").addClass("sidebar-closed");
                    $(".navbar").addClass("expand-header");
                    $('.overlay').addClass('show');
                    $('#container').addClass('sbar-open');
                    $('.sidebar-wrapper [aria-expanded="true"]').parents('li.menu').find('.collapse').removeClass('show');
                }
            }

        }

        function sidebarMobCheck() {
            if (window.innerWidth <= 1199 ) {

                if ( $('.main-container').hasClass('sbar-open') || $('#compact_submenuSidebar').hasClass('show') ) {
                    return;
                } else {
                    sidebarCloser()
                }
            } else if (window.innerWidth > 1199 ) {
                sidebarCloser();
            }
        }

        sidebarCloser();

        $(window).resize(function(event) {
            sidebarMobCheck();
        });

    }


    return {
        init: function () {

            /*
                GrayDuck Software
                Local storage has the token data we can use to verify the token is indeed correct and valid.
            */
            var grayduckAUTH;
            var varSubscriptions = '';
            if (localStorage.getItem('grayduckAUTH') === null) {
                //Do Nothing
                window.location.replace("auth_login.html");
            } else {
                //Parse JSon Object
                grayduckAUTH = JSON.parse(localStorage.getItem('grayduckAUTH'));
                //Set values
                document.getElementById('spanUserProfileFullName').innerHTML = 'Connected';
                document.getElementById('spanUserProfileEmail').innerHTML = grayduckAUTH.email;

                //Load Subscriptions that the user has access to
                //document.getElementById("spanSubscriptions").innerHTML = '';
                grayduckAUTH.authSecurity.forEach(
                    element => {
                        varSubscriptions += '<a class="dropdown-item"><div class=""><div class="media"><div class="user-img"><div class="avatar avatar-xl"><span class="avatar-title rounded-circle">SAAS</span></div></div><div class="media-body"><div class=""><h5 class="usr-name">Subscription</h5><p class="msg-title">' + element.subscriptionId + '</p></div></div></div></div></a>'
                    }
                );
                document.getElementById("spanSubscriptions").innerHTML = varSubscriptions;
            }



            /*
                GrayDuck Software
                Load SideBar Main Menus and User Profile Info
            */
            LoadMainMenus();

            /*
                Standard functions
            */
            toggleFunction.overlay();
            toggleFunction.search();
            toggleFunction.navbarShadow();

            /*
                Desktop Resoltion fn
            */
            _desktopResolution.onRefresh();
            _desktopResolution.onResize();

            /*
                Mobile Resoltion fn
            */
            _mobileResolution.onRefresh();            
            _mobileResolution.onResize();

            sidebarFunctionality();

            /*
                In Built Functionality fn
            */
            inBuiltfunctionality.subCatScroll();
            inBuiltfunctionality.preventScrollBody();
            inBuiltfunctionality.languageDropdown();
            inBuiltfunctionality.onSidebarClick();


        }
    }

}();
