document.addEventListener('DOMContentLoaded', function() {
  console.log('DOM fully loaded and parsed');


  // Add IDs to navbar items. Unfortunately this is not done by 
  const navLinks = document.querySelectorAll('.nav-link');
  navLinks.forEach(link => {
    if (link.textContent.trim() === 'Handleiding') {
      link.id = 'handleiding-link';
    } else if (link.textContent.trim() === 'Implementatie Rijnland') {
      link.id = 'rijnland-link';
    }
  });

  // Verify IDs have been added
  console.log(document.getElementById('handleiding-link'));
  console.log(document.getElementById('rijnland-link'));

  // Function to hide all sidebar sections
  function hideAllSidebarSections() {
    document.querySelectorAll('.sidebar-section').forEach(function(section) {
      section.style.display = 'none';
    });
  }

  // Function to show specific sidebar section
  function showSidebarSection(sectionId) {
    const section = document.getElementById(sectionId);
    if (section) {
      section.style.display = 'block';
    } else {
      console.log('Section not found: ' + sectionId);
    }
  }

  // Add event listeners to navbar items
  const handleidingLink = document.getElementById('handleiding-link');
  const rijnlandLink = document.getElementById('rijnland-link');

  if (handleidingLink) {
    handleidingLink.addEventListener('click', function(event) {
      event.preventDefault(); // Prevent default link behavior
      console.log('Handleiding clicked');
      hideAllSidebarSections();
      showSidebarSection('quarto-sidebar-section-1'); // Gebruikershandleiding section
      showSidebarSection('quarto-sidebar-section-3'); // Technische documentatie section
      showSidebarSection('quarto-sidebar-section-4'); // Voorbewerkingen section
      showSidebarSection('quarto-sidebar-section-5'); // Nabewerkingen section
    });
  } else {
    console.log('Handleiding link not found');
  }

  if (rijnlandLink) {
    rijnlandLink.addEventListener('click', function(event) {
      event.preventDefault(); // Prevent default link behavior
      console.log('Rijnland clicked');
      hideAllSidebarSections();
      showSidebarSection('quarto-sidebar-section-2'); // Risicofactoren section
    });
  } else {
    console.log('Rijnland link not found');
  }

  // Optionally, set the default sidebar state
  hideAllSidebarSections();
  showSidebarSection('quarto-sidebar-section-1'); // Default to Gebruikershandleiding section
});
