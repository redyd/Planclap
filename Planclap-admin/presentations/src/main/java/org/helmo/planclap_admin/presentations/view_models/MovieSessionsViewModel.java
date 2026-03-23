package org.helmo.planclap_admin.presentations.view_models;

import java.util.List;

public record MovieSessionsViewModel(String title, List<String> cinechecks, String poster, String duration, String description, int sessionsAmount) {


}
